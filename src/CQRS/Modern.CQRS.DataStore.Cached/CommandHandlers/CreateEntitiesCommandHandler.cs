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
/// The mediator command handler that creates a list of new entities in the data store
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entities in the data store</exception>
/// <returns>A list of updated entities by the data store (primary key, for example)</returns>
public class CreateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<CreateEntitiesCommand<TEntityDto>, List<TEntityDto>>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernCrudRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(CreateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public CreateEntitiesCommandHandler(TRepository repository, IModernCache<TEntityDto, TId> cache,
        ILogger<CreateEntitiesCommandHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
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
    public async Task<List<TEntityDto>> Handle(CreateEntitiesCommand<TEntityDto> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Entities, nameof(request.Entities));
            Guard.Against.NegativeOrZero(request.Entities.Count, nameof(request.Entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", EntityName, HandlerName, request.Entities);

            var entitiesDbo = request.Entities.ConvertAll(MapToDbo);

            Logger.LogDebug("Creating {name} entities in db...", EntityName);
            entitiesDbo = await Repository.CreateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entities. {@entityDbo}", EntityName, entitiesDbo);

            var entitiesDto = entitiesDbo.ConvertAll(MapToDto);
            var dictionary = entitiesDto.ToDictionary(key => GetEntityId(key), value => value);

            Logger.LogDebug("Creating {name} entities in cache...", EntityName);
            await Cache.AddOrUpdateAsync(dictionary).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entities. {@entityDto}", EntityName, entitiesDto);

            return entitiesDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create new {name} entities: {reason}. {@entities}", EntityName, ex.Message, request.Entities);
            throw CreateProperException(ex);
        }
    }
}
