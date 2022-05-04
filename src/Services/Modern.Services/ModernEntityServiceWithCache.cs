using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using MapsterMapper;
using Modern.Cache.Abstractions;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Exceptions;
using Modern.Services.Abstractions;
using Modern.Services.Abstractions.Query;

namespace Modern.Services;

/// <summary>
/// Represents an <see cref="IModernEntityService{TEntityDto, TEntityDbo, TId}"/> implementation
/// with data access through generic repository with distributed caching
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
/// <typeparam name="TRepository">Type of repository used for the entity</typeparam>
public class ModernEntityServiceWithCache<TEntityDto, TEntityDbo, TId, TRepository> :
    IModernEntityService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>, IModernCrudRepository<TEntityDbo, TId>
{
    private readonly string _entityName = typeof(TEntityDto).Name;
    private readonly string _serviceName = $"{typeof(TEntityDto).Name}Service";
    private readonly IMapper _mapper = new Mapper();

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly TRepository Repository;

    /// <summary>
    /// The distributed cache
    /// </summary>
    protected readonly IModernDistributedCache<TEntityDto, TId> DistributedCache;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="distributedCache">Distributed cache</param>
    /// <param name="logger">The logger</param>
    public ModernEntityServiceWithCache(TRepository repository, IModernDistributedCache<TEntityDto, TId> distributedCache,
        ILogger<ModernEntityServiceWithCache<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        DistributedCache = distributedCache;
        Logger = logger;
    }

    /// <summary>
    /// Returns <typeparamref name="TEntityDto"/> mapped from <typeparamref name="TEntityDbo"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity Dbo</returns>
    protected virtual TEntityDbo MapToDbo(TEntityDto entityDto) => _mapper.Map<TEntityDbo>(entityDto);

    /// <summary>
    /// Returns <typeparamref name="TEntityDbo"/> mapped from <typeparamref name="TEntityDto"/>
    /// </summary>
    /// <param name="entityDbo">Entity Dbo</param>
    /// <returns>Entity Dto</returns>
    protected virtual TEntityDto MapToDto(TEntityDbo entityDbo) => _mapper.Map<TEntityDto>(entityDbo);

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity id</returns>
    // TODO: use source generators for this
    protected virtual TId GetEntityId(TEntityDto entityDto) => (TId)(entityDto.GetType().GetProperty("Id")?.GetValue(entityDto, null) ?? 0);

    /// <summary>
    /// Returns standardized service exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
        => ex switch
        {
            ArgumentException _ => ex,
            EntityConcurrentUpdateException _ => ex,
            EntityAlreadyExistsException _ => ex,
            EntityNotFoundException _ => ex,
            EntityNotModifiedException _ => ex,
            _ => new RepositoryErrorException(ex.Message, ex)
        };

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(GetByIdAsync), id);
            }

            var entityDto = await DistributedCache.TryGetByIdAsync(id).ConfigureAwait(false);
            if (entityDto is not null)
            {
                return entityDto;
            }

            var entityDbo = await Repository.GetByIdAsync(id, null, cancellationToken).ConfigureAwait(false);
            
            entityDto = MapToDto(entityDbo);
            await DistributedCache.AddOrUpdateAsync(GetEntityId(entityDto), entityDto).ConfigureAwait(false);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.TryGetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(TryGetByIdAsync), id);
            }

            var entityDto = await DistributedCache.TryGetByIdAsync(id).ConfigureAwait(false);
            if (entityDto is not null)
            {
                return entityDto;
            }

            var entityDbo = await Repository.TryGetByIdAsync(id, null, cancellationToken).ConfigureAwait(false);
            if (entityDbo is null)
            {
                return null;
            }

            entityDto = MapToDto(entityDbo);
            await DistributedCache.AddOrUpdateAsync(GetEntityId(entityDto), entityDto).ConfigureAwait(false);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.GetAllAsync"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(GetAllAsync));

            var entitiesDbo = await Repository.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
            return entitiesDbo.ConvertAll(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get all {name} entities: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.CountAsync(CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} of all entities", _serviceName, nameof(CountAsync));
            }

            return await Repository.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get count of all {name} entities: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.CountAsync(Expression{Func{TEntityDbo, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(CountAsync));

            return await Repository.CountAsync(predicate, null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities count by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.ExistsAsync"/>
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(ExistsAsync));

            return await Repository.ExistsAsync(predicate, null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not check {name} entity existence by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.FirstOrDefaultAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto?> FirstOrDefaultAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(FirstOrDefaultAsync));

            var entityDbo = await Repository.FirstOrDefaultAsync(predicate, null, cancellationToken).ConfigureAwait(false);
            return entityDbo is not null ? MapToDto(entityDbo) : null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get first {name} entity by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.SingleOrDefaultAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto?> SingleOrDefaultAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(SingleOrDefaultAsync));

            var entityDbo = await Repository.SingleOrDefaultAsync(predicate, null, cancellationToken).ConfigureAwait(false);
            return entityDbo is not null ? MapToDto(entityDbo) : null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get single {name} entity by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.WhereAsync(Expression{Func{TEntityDbo, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(WhereAsync));

            var entitiesDbo = await Repository.WhereAsync(predicate, null, cancellationToken).ConfigureAwait(false);
            return entitiesDbo.ConvertAll(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.WhereAsync(Expression{Func{TEntityDbo, bool}},int,int,CancellationToken)"/>
    /// </summary>
    public virtual async Task<PagedResult<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(WhereAsync));

            var pagedResult = await Repository.WhereAsync(predicate, pageNumber, pageSize, null, cancellationToken).ConfigureAwait(false);
            return new PagedResult<TEntityDto>
            {
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items.ToList().ConvertAll(MapToDto)
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto, TEntityDbo,TId}.AsQueryable"/>
    /// </summary>
    public virtual IQueryable<TEntityDbo> AsQueryable()
    {
        try
        {
            LogMethod(nameof(AsQueryable));

            return Repository.AsQueryable();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities as Queryable: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.CreateAsync(TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entity: {@entity}", _serviceName, nameof(CreateAsync), entity);

            var entityDbo = MapToDbo(entity);

            Logger.LogDebug("Creating {name} entity in db...", _entityName);
            entityDbo = await Repository.CreateAsync(entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entity. {@entityDbo}", _entityName, entityDbo);

            var entityDto = MapToDto(entityDbo);
            var entityId = GetEntityId(entityDto);

            Logger.LogDebug("Creating {name} entity with id '{id}' in cache...", _entityName, entityId);
            await DistributedCache.AddOrUpdateAsync(entityId, entityDto).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entity with id '{id}'. {@entityDto}", _entityName, entityId, entityDto);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create a new {name} entity: {reason}. {@entity}", _entityName, ex.Message, entity);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.CreateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> CreateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", _serviceName, nameof(CreateAsync), entities);

            var entitiesDbo = entities.ConvertAll(MapToDbo);

            Logger.LogDebug("Creating {name} entities in db...", _entityName);
            entitiesDbo = await Repository.CreateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entities. {@entityDbo}", _entityName, entitiesDbo);

            // TODO: AddMany
            var entitiesDto = entitiesDbo.ConvertAll(MapToDto);
            foreach (var entityDto in entitiesDto)
            {
                var entityId = GetEntityId(entityDto);

                Logger.LogDebug("Creating {name} entity with id '{id}' in cache...", _entityName, entityId);
                await DistributedCache.AddOrUpdateAsync(entityId, entityDto).ConfigureAwait(false);
                Logger.LogDebug("Created {name} entities with id '{id}'. {@entityDto}", _entityName, entityId, entityDto);
            }

            return entitiesDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create new {name} entities: {reason}. {@entities}", _entityName, ex.Message, entities);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> UpdateAsync(TId id, TEntityDto entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} id: {id}, entity: {@entity}", _serviceName, nameof(UpdateAsync), id, entity);

            var entityDbo = MapToDbo(entity);

            Logger.LogDebug("Updating {name} entity with id '{id}' in db...", _entityName, id);
            entityDbo = await Repository.UpdateAsync(id, entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id {id}. {@entityDbo}", _entityName, id, entityDbo);

            var entityDto = MapToDto(entityDbo);
            var entityId = GetEntityId(entityDto);

            Logger.LogDebug("Updating {name} entity with id '{id}' in cache...", _entityName, entityId);
            await DistributedCache.AddOrUpdateAsync(entityId, entityDto).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id '{id}'. {@entityDto}", _entityName, entityId, entityDto);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update a {name} entity by id '{id}': {reason}. {@entity}", _entityName, id, ex.Message, entity);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> UpdateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", _serviceName, nameof(UpdateAsync), entities);

            var entitiesDbo = entities.ConvertAll(MapToDbo);

            Logger.LogDebug("Updating entity in db...");
            entitiesDbo = await Repository.UpdateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entities. {@entitiesDbo}", _entityName, entitiesDbo);

            var entitiesDto = entitiesDbo.ConvertAll(MapToDto);
            var dictionary = entitiesDto.ToDictionary(key => GetEntityId(key), value => value);

            Logger.LogDebug("Updating {name} entities from cache with ids: {@ids}...", _entityName, dictionary);
            await DistributedCache.AddOrUpdateAsync(dictionary).ConfigureAwait(false);
            Logger.LogDebug("Update {name} entities", _entityName);

            return entitiesDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update {name} entities: {reason}. {@entities}", _entityName, ex.Message, entities);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,Action{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> UpdateAsync(TId id, Action<TEntityDbo> update, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(update, nameof(update));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(UpdateAsync), id);

            Logger.LogDebug("Updating {name} entity with id '{id}' in db...", _entityName, id);
            var entityDbo = await Repository.UpdateAsync(id, update, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id {id}. {@entityDbo}", _entityName, id, entityDbo);

            var entityDto = MapToDto(entityDbo);
            var entityId = GetEntityId(entityDto);

            Logger.LogDebug("Updating {name} entity with id '{id}' in cache...", _entityName, entityId);
            await DistributedCache.AddOrUpdateAsync(entityId, entityDto).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id '{id}'. {@entityDto}", _entityName, entityId, entityDto);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update a {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(DeleteAsync), id);

            Logger.LogDebug("Deleting {name} entity with id '{id}' in db...", _entityName, id);
            await Repository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id {id}", _entityName, id);

            Logger.LogDebug("Deleting {name} entity with id '{id}' from cache...", _entityName, id);
            await DistributedCache.DeleteAsync(id).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id '{id}'", _entityName, id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete a {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(List{TId},CancellationToken)"/>
    /// </summary>
    public virtual async Task DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} ids: {@ids}", _serviceName, nameof(DeleteAsync), ids);

            Logger.LogDebug("Updating {name} entities in db...", _entityName);
            await Repository.DeleteAsync(ids, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entities with ids: {@ids}", _entityName, ids);

            Logger.LogDebug("Deleting {name} entities from cache with ids: {@ids}...", _entityName, ids);
            await DistributedCache.DeleteAsync(ids).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entities", _entityName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete {name} entities by ids '{@ids}': {reason}", _entityName, ids, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAndReturnAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(DeleteAndReturnAsync), id);

            Logger.LogDebug("Deleting {name} entity with id '{id}' in db...", _entityName, id);
            var entityDbo = await Repository.DeleteAndReturnAsync(id, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id {id}. {@entityDbo}", _entityName, id, entityDbo);

            var entityDto = await DistributedCache.GetByIdAsync(id).ConfigureAwait(false);

            Logger.LogDebug("Deleting {name} entity with id '{id}' from cache...", _entityName, id);
            await DistributedCache.DeleteAsync(id).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id '{id}'", _entityName, id);

            return entityDto;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete a {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    private void LogMethod(string methodName)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
        {
            Logger.LogTrace("{serviceName}.{method}", _serviceName, methodName);
        }
    }
}