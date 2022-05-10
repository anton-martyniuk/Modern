using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstract;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.CommandHandlers;

/// <summary>
/// The mediator command handler that updates the list of entities in the data store with the given list of entities.<br/>
/// If all or some of entities were not found in the data store - no exception is thrown
/// </summary>
/// <returns>List of updated entities</returns>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
/// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entities in the data store</exception>
public class UpdateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo>,
    IRequestHandler<UpdateEntitiesCommand<TEntityDto>, List<TEntityDto>>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernCrudRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(UpdateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public UpdateEntitiesCommandHandler(TRepository repository, ILogger<UpdateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<List<TEntityDto>> Handle(UpdateEntitiesCommand<TEntityDto> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Entities, nameof(request.Entities));
            Guard.Against.NegativeOrZero(request.Entities.Count, nameof(request.Entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", EntityName, HandlerName, request.Entities);

            var entitiesDbo = request.Entities.ConvertAll(MapToDbo);

            Logger.LogDebug("Updating entity in db...");
            entitiesDbo = await Repository.UpdateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entities. {@entitiesDbo}", EntityName, entitiesDbo);

            return entitiesDbo.ConvertAll(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update {name} entities: {reason}. {@entities}", EntityName, ex.Message, request.Entities);
            throw CreateProperException(ex);
        }
    }
}
