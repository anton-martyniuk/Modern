using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Mapster;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.Abstractions;

namespace Modern.Services.DataStore;

/// <summary>
/// Represents an <see cref="IModernService{TEntityDto,TEntityDbo,TId}"/> implementation
/// with data access through generic repository
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernService<TEntityDto, TEntityDbo, TId> :
    IModernService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly string _entityName = typeof(TEntityDto).Name;
    private readonly string _serviceName = $"{typeof(TEntityDto).Name}Service";

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly IModernRepository<TEntityDbo, TId> Repository;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="logger">The logger</param>
    public ModernService(IModernRepository<TEntityDbo, TId> repository, ILogger<ModernService<TEntityDto, TEntityDbo, TId>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// Returns <typeparamref name="TEntityDto"/> mapped from <typeparamref name="TEntityDbo"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity Dbo</returns>
    protected virtual TEntityDbo MapToDbo(TEntityDto entityDto)
    {
        if (typeof(TEntityDbo) == typeof(TEntityDto))
        {
            return (TEntityDbo)(object)entityDto;
        }

        return entityDto.Adapt<TEntityDbo>();
    }

    /// <summary>
    /// Returns <typeparamref name="TEntityDbo"/> mapped from <typeparamref name="TEntityDto"/>
    /// </summary>
    /// <param name="entityDbo">Entity Dbo</param>
    /// <returns>Entity Dto</returns>
    protected virtual TEntityDto MapToDto(TEntityDbo entityDbo)
    {
        if (typeof(TEntityDbo) == typeof(TEntityDto))
        {
            return (TEntityDto)(object)entityDbo;
        }

        return entityDbo.Adapt<TEntityDto>();
    }

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
            RepositoryErrorException _ => ex,
            TaskCanceledException _ => ex,
            _ => new InternalErrorException(ex.Message, ex)
        };

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{ServiceName}.{Method} id: {Id}", _serviceName, nameof(GetByIdAsync), id);
            }

            var entityDbo = await Repository.GetByIdAsync(id, null, cancellationToken).ConfigureAwait(false);
            return MapToDto(entityDbo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {Name} entity by id '{Id}': {Reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.TryGetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{ServiceName}.{Method} id: {Id}", _serviceName, nameof(TryGetByIdAsync), id);
            }

            var entityDbo = await Repository.TryGetByIdAsync(id, null, cancellationToken).ConfigureAwait(false);
            return entityDbo is not null ? MapToDto(entityDbo) : null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {Name} entity by id '{Id}': {Reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.GetAllAsync"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(GetAllAsync));

            var entitiesDbo = await Repository.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
            return entitiesDbo.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get all {Name} entities: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.CountAsync(CancellationToken)"/>
    /// </summary>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{ServiceName}.{Method} of all entities", _serviceName, nameof(CountAsync));
            }

            return await Repository.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get count of all {Name} entities: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.CountAsync(Expression{Func{TEntityDbo, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<long> CountAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
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
            Logger.LogError(ex, "Could not get {Name} entities count by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.ExistsAsync"/>
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
            Logger.LogError(ex, "Could not check {Name} entity existence by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.FirstOrDefaultAsync"/>
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
            Logger.LogError(ex, "Could not get first {Name} entity by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.SingleOrDefaultAsync"/>
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
            Logger.LogError(ex, "Could not get single {Name} entity by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.WhereAsync(Expression{Func{TEntityDbo, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            LogMethod(nameof(WhereAsync));

            var entitiesDbo = await Repository.WhereAsync(predicate, null, cancellationToken).ConfigureAwait(false);
            return entitiesDbo.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {Name} entities by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.WhereAsync(Expression{Func{TEntityDbo, bool}},int,int,CancellationToken)"/>
    /// </summary>
    public virtual async Task<PagedResult<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{ServiceName}.{Method}. Page number: {PageNumber}, page size: {PageSize}", _serviceName, nameof(WhereAsync), pageNumber, pageSize);
            }

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
            Logger.LogError(ex, "Could not get {Name} entities by the given predicate: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntityDto,TEntityDbo,TId}.AsQueryable"/>
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
            Logger.LogError(ex, "Could not get {Name} entities as Queryable: {Reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.CreateAsync(TEntityDto,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} entity: {@Entity}", _serviceName, nameof(CreateAsync), entity);

            Logger.LogDebug("Creating {Name} entity in db...", _entityName);
            var entityDbo = MapToDbo(entity);
            entityDbo = await Repository.CreateAsync(entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {Name} entity. {@EntityDbo}", _entityName, entityDbo);

            return MapToDto(entityDbo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create a new {Name} entity: {Reason}. {@Entity}", _entityName, ex.Message, entity);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.CreateAsync(List{TEntityDto},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> CreateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} entities: {@Entities}", _serviceName, nameof(CreateAsync), entities);

            Logger.LogDebug("Creating {Name} entities in db...", _entityName);
            var entitiesDbo = entities.ConvertAll(MapToDbo);
            entitiesDbo = await Repository.CreateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {Name} entities. {@EntityDbo}", _entityName, entitiesDbo);

            return entitiesDbo.ConvertAll(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to create new {Name} entities: {Reason}. {@Entities}", _entityName, ex.Message, entities);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.UpdateAsync(TId,TEntityDto,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> UpdateAsync(TId id, TEntityDto entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} id: {Id}, entity: {@Entity}", _serviceName, nameof(UpdateAsync), id, entity);

            var entityDbo = MapToDbo(entity);

            Logger.LogDebug("Updating {Name} entity with id '{Id}' in db...", _entityName, id);
            entityDbo = await Repository.UpdateAsync(id, entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {Name} entity with id {Id}. {@EntityDbo}", _entityName, id, entityDbo);

            return MapToDto(entityDbo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update a {Name} entity by id '{Id}': {Reason}. {@Entity}", _entityName, id, ex.Message, entity);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.UpdateAsync(List{TEntityDto},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntityDto>> UpdateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} entities: {@Entities}", _serviceName, nameof(UpdateAsync), entities);

            var entitiesDbo = entities.ConvertAll(MapToDbo);

            Logger.LogDebug("Updating entity in db...");
            entitiesDbo = await Repository.UpdateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {Name} entities. {@EntitiesDbo}", _entityName, entitiesDbo);

            return entitiesDbo.ConvertAll(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update {Name} entities: {Reason}. {@Entities}", _entityName, ex.Message, entities);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.UpdateAsync(TId,Action{TEntityDto},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntityDto> UpdateAsync(TId id, Action<TEntityDto> update, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(update, nameof(update));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} id: {Id}", _serviceName, nameof(UpdateAsync), id);

            var entityDbo = await Repository.GetByIdAsync(id, null, cancellationToken).ConfigureAwait(false);
            var entityDto = MapToDto(entityDbo);

            // Perform update action
            update(entityDto);

            Logger.LogDebug("Updating {Name} entity with id '{Id}' in db...", _entityName, id);
            entityDbo = MapToDbo(entityDto);
            entityDbo = await Repository.UpdateAsync(id, entityDbo, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {Name} entity with id {Id}. {@EntityDbo}", _entityName, id, entityDbo);

            return MapToDto(entityDbo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update a {Name} entity by id '{Id}': {Reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} id: {Id}", _serviceName, nameof(DeleteAsync), id);
            Logger.LogDebug("Deleting {Name} entity with id '{Id}' in db...", _entityName, id);

            var result = await Repository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                Logger.LogDebug("{Name} entity with id {Id} was not found for deletion", _entityName, id);
                return result;
            }

            Logger.LogDebug("Deleted {Name} entity with id {Id}", _entityName, id);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete a {Name} entity by id '{Id}': {Reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.DeleteAsync(List{TId},CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            Guard.Against.NegativeOrZero(ids.Count, nameof(ids));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} ids: {@Ids}", _serviceName, nameof(DeleteAsync), ids);
            Logger.LogDebug("Updating {Name} entities in db...", _entityName);

            var result = await Repository.DeleteAsync(ids, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                Logger.LogDebug("Not all {Name} entities with ids: {@Ids} were found for deletion", _entityName, ids);
                return result;
            }

            Logger.LogDebug("Deleted {Name} entities with ids: {@Ids}. Result: {Result}", _entityName, ids, result);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete {Name} entities by ids '{@Ids}': {Reason}", _entityName, ids, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudService{TEntityDto,TId}.DeleteAndReturnAsync"/>
    /// </summary>
    public virtual async Task<TEntityDto> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogTrace("{ServiceName}.{Method} id: {Id}", _serviceName, nameof(DeleteAndReturnAsync), id);

            Logger.LogDebug("Deleting {Name} entity with id '{Id}' in db...", _entityName, id);
            var entityDbo = await Repository.DeleteAndReturnAsync(id, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {Name} entity with id {Id}. {@EntityDbo}", _entityName, id, entityDbo);

            return MapToDto(entityDbo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete a {Name} entity by id '{Id}': {Reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    private void LogMethod(string methodName)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
        {
            Logger.LogTrace("{ServiceName}.{Method}", _serviceName, methodName);
        }
    }
}