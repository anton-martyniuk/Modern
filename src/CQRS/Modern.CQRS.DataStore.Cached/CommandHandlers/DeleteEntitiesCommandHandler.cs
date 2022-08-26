using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Modern.Cache.Abstractions;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.CommandHandlers;

/// <summary>
/// The mediator command handler that deletes the list of entities in the data store with the given list of entity ids.<br/>
/// This method does NOT query the entities from the data store before deletion.<br/>
/// If all or some of entities were not found in the data store - no exception is thrown
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entities in the data store</exception>
/// <returns><see langword="true"/> if all entities were deleted; otherwise, <see langword="false"/></returns>
public class DeleteEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<DeleteEntitiesCommand<TId>, bool>
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
    /// The cache
    /// </summary>
    protected readonly IModernCache<TEntityDto, TId> Cache;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="cache">Cache</param>
    /// <param name="logger">The logger</param>
    public DeleteEntitiesCommandHandler(TRepository repository, IModernCache<TEntityDto, TId> cache,
        ILogger<DeleteEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Cache = cache;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<bool> Handle(DeleteEntitiesCommand<TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Ids, nameof(request.Ids));
            Guard.Against.NegativeOrZero(request.Ids.Count, nameof(request.Ids));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} ids: {@ids}", EntityName, HandlerName, request.Ids);
            Logger.LogDebug("Updating {name} entities in db...", EntityName);

            var result = await Repository.DeleteAsync(request.Ids, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                Logger.LogDebug("Not all {name} entities with ids: {@ids} were found for deletion", EntityName, request.Ids);
                return result;
            }

            Logger.LogDebug("Deleted {name} entities with ids: {@ids}. Result: {result}", EntityName, request.Ids, result);

            Logger.LogDebug("Deleting {name} entities from cache with ids: {@ids}...", EntityName, request.Ids);
            await Cache.DeleteAsync(request.Ids).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entities", EntityName);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete {name} entities by ids '{@ids}': {reason}", EntityName, request.Ids, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
