using MediatR;
using Microsoft.Extensions.Logging;
using Modern.Cache.Abstractions;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.CommandHandlers;

/// <summary>
/// The mediator command handler that updates the entity in the data store with the given entity id
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided id or entity is null</exception>
/// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
/// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entity in the data store</exception>
/// <returns>Updated entity</returns>
public class UpdateEntityByActionCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<UpdateEntityByActionCommand<TEntityDto, TId>, TEntityDto>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(UpdateEntityByActionCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public UpdateEntityByActionCommandHandler(TRepository repository, IModernCache<TEntityDto, TId> cache,
        ILogger<UpdateEntityByActionCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
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
    public async Task<TEntityDto> Handle(UpdateEntityByActionCommand<TEntityDto, TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Id, nameof(request.Id));
            ArgumentNullException.ThrowIfNull(request.UpdateAction, nameof(request.UpdateAction));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} id: {id}", EntityName, HandlerName, request.Id);

            var entityDbo = await Repository.GetByIdAsync(request.Id, null, cancellationToken).ConfigureAwait(false);
            var entityDto = MapToDto(entityDbo);

            // Perform update action
            request.UpdateAction(entityDto);

            Logger.LogDebug("Updating {name} entity with id '{id}' in db...", EntityName, request.Id);
            entityDbo = MapToDbo(entityDto);
            entityDbo = await Repository.UpdateAsync(request.Id, entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id {id}. {@entityDbo}", EntityName, request.Id, entityDbo);

            entityDto = MapToDto(entityDbo);

            Logger.LogDebug("Updating {name} entity with id '{id}' in cache...", EntityName, request.Id);
            await Cache.AddOrUpdateAsync(request.Id, entityDto).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id '{id}'. {@entityDto}", EntityName, request.Id, entityDto);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update a {name} entity by id '{id}': {reason}", EntityName, request.Id, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
