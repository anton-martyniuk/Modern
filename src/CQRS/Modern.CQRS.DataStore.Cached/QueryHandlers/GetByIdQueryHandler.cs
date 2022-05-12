using MediatR;
using Microsoft.Extensions.Logging;
using Modern.Cache.Abstractions;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.QueryHandlers;

/// <summary>
/// The mediator query handler that returns an entity with the given id
/// </summary>
/// <returns>The entity</returns>
/// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
/// <exception cref="EntityNotFoundException">Thrown if an entity does is not found</exception>
/// <exception cref="InternalErrorException">If a service internal error occurred</exception>
public class GetByIdQueryHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<GetByIdQuery<TEntityDto, TId>, TEntityDto>

    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(GetByIdQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public GetByIdQueryHandler(TRepository repository, IModernCache<TEntityDto, TId> cache,
        ILogger<GetByIdQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
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
    public async Task<TEntityDto> Handle(GetByIdQuery<TEntityDto, TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Id, nameof(request.Id));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} id: {id}", EntityName, HandlerName, request.Id);
            }

            var entityDto = await Cache.TryGetByIdAsync(request.Id).ConfigureAwait(false);
            if (entityDto is not null)
            {
                return entityDto;
            }

            var entityDbo = await Repository.GetByIdAsync(request.Id, null, cancellationToken).ConfigureAwait(false);

            entityDto = MapToDto(entityDbo);
            await Cache.AddOrUpdateAsync(GetEntityId(entityDto), entityDto).ConfigureAwait(false);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entity by id '{id}': {reason}", EntityName, request.Id, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
