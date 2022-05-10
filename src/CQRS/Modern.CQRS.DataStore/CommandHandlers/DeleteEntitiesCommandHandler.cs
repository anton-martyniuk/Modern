using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstract;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.CommandHandlers;

/// <summary>
/// The mediator command handler that deletes the list of entities in the data store with the given list of entity ids.<br/>
/// This method does NOT query the entities from the data store before deletion.<br/>
/// If all or some of entities were not found in the data store - no exception is thrown
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entities in the data store</exception>
public class DeleteEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo>,
    IRequestHandler<DeleteEntitiesCommand<TId>>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernCrudRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(DeleteEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>);

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly TRepository Repository;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="logger">The logger</param>
    public DeleteEntitiesCommandHandler(TRepository repository, ILogger<DeleteEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<Unit> Handle(DeleteEntitiesCommand<TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Ids, nameof(request.Ids));
            Guard.Against.NegativeOrZero(request.Ids.Count, nameof(request.Ids));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} ids: {@ids}", EntityName, HandlerName, request.Ids);

            Logger.LogDebug("Updating {name} entities in db...", EntityName);
            await Repository.DeleteAsync(request.Ids, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entities with ids: {@ids}", EntityName, request.Ids);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete {name} entities by ids '{@ids}': {reason}", EntityName, request.Ids, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
