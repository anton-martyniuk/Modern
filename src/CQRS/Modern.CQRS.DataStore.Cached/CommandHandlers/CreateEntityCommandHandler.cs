using MediatR;
using Microsoft.Extensions.Logging;
using Modern.Cache.Abstractions;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.CommandHandlers;

/// <summary>
/// The mediator command handler that creates the new entity
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided entity is null</exception>
/// <exception cref="EntityAlreadyExistsException">Thrown if an entity already exists in the data store</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entity in the data store</exception>
/// <returns>Updated entity by the data store (primary key, for example)</returns>
public class CreateEntityCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<CreateEntityCommand<TEntityDto>, TEntityDto>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernCrudRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(CreateEntityCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public CreateEntityCommandHandler(TRepository repository, IModernCache<TEntityDto, TId> cache,
        ILogger<CreateEntityCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
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
    public async Task<TEntityDto> Handle(CreateEntityCommand<TEntityDto> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Entity, nameof(request.Entity));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entity: {@entity}", EntityName, HandlerName, request.Entity);

            var entityDbo = MapToDbo(request.Entity);

            Logger.LogDebug("Creating {name} entity in db...", EntityName);
            entityDbo = await Repository.CreateAsync(entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entity. {@entityDbo}", EntityName, entityDbo);

            var entityDto = MapToDto(entityDbo);
            var entityId = GetEntityId(entityDto);

            Logger.LogDebug("Creating {name} entity with id '{id}' in cache...", EntityName, entityId);
            await Cache.AddOrUpdateAsync(entityId, entityDto).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entity with id '{id}'. {@entityDto}", EntityName, entityId, entityDto);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create a new {name} entity: {reason}. {@entity}", EntityName, ex.Message, request.Entity);
            throw CreateProperException(ex);
        }
    }
}
