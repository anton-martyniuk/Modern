using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Exceptions;
using Modern.Services.Abstractions;

namespace Modern.Services;

/// <summary>
/// Represents an <see cref="IModernCrudService{TEntity,TId}"/> and <see cref="IModernQueryService{TEntity,TId}"/> implementation
/// with data access through generic repository
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
/// <typeparam name="TRepository">Type of repository used for the entity</typeparam>
public abstract class ModernService<TEntity, TId, TRepository> : IModernCrudService<TEntity, TId>, IModernQueryService<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntity, TId>, IModernCrudRepository<TEntity, TId>
{
    private readonly string _entityName = typeof(TEntity).Name;
    private readonly string _serviceName = $"{typeof(TEntity).Name}Service";

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
    protected ModernService(TRepository repository, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
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
            _ => new RepositoryErrorException(ex.Message, ex)
        };

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(GetByIdAsync), id);
            }

            return await Repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.TryGetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(TryGetByIdAsync), id);
            }

            return await Repository.TryGetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entity by id '{id}': {reason}", _entityName, id, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.GetAllAsync"/>
    /// </summary>
    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            LogMethod(nameof(GetAllAsync));

            return await Repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get all {name} entities: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.CountAsync(CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
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
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.CountAsync(Expression{Func{TEntity, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            LogMethod(nameof(CountAsync));

            return await Repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities count by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.ExistsAsync"/>
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            LogMethod(nameof(ExistsAsync));

            return await Repository.ExistsAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not check {name} entity existence by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.FirstOrDefaultAsync"/>
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            LogMethod(nameof(FirstOrDefaultAsync));

            return await Repository.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get first {name} entity by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.SingleOrDefaultAsync"/>
    /// </summary>
    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            LogMethod(nameof(SingleOrDefaultAsync));

            return await Repository.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get single {name} entity by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.WhereAsync"/>
    /// </summary>
    public virtual async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            LogMethod(nameof(WhereAsync));

            return await Repository.WhereAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities by the given predicate: {reason}", _entityName, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryService{TEntity,TId}.AsQueryable"/>
    /// </summary>
    public IQueryable<TEntity> AsQueryable()
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
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            Logger.LogTrace("{serviceName}.{method} entity: {@entity}", _serviceName, nameof(CreateAsync), entity);

            // TODO: TEntity in service should be DTO or DBO ?!

            // TODO: accept TEntityDto and TEntityDbo in Service template arguments

            // TODO: Service with and without cache

            Logger.LogDebug("Creating {name} entity in db...", _entityName);
            var dboEntity = await Repository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created {name} entity. {@dboEntity}", _entityName, dboEntity);

            //var dboEntity = MapDtoToDbo(entity);

            //Logger.LogDebug("Creating entity in db...");
            //await Repository.CreateAsync(dboEntity, cancellationToken).ConfigureAwait(false);
            //Logger.LogDebug($"Created. Id: {dboEntity.Id}");

            //Logger.LogDebug("Adding entity to cache...");
            //var entityDto = MapDtoToDbo(dboEntity);
            //_cache.AddOrUpdate(entityDto.Id, entityDto);
            //Logger.LogDebug("Added.");

            return dboEntity;
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
    public async Task<List<TEntity>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", _serviceName, nameof(CreateAsync), entities);

            Logger.LogDebug("Creating {name} entities in db...", _entityName);
            var dboEntities = await Repository.CreateAsync(entities, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Created many {name} entities. {@dboEntity}", _entityName, dboEntities);

            return dboEntities;
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
    public async Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            Logger.LogTrace("{serviceName}.{method} id: {id}, entity: {@entity}", _serviceName, nameof(UpdateAsync), id, entity);

            Logger.LogDebug("Updating {name} entity with id '{id}' in db...", _entityName, id);
            var dboEntity = await Repository.UpdateAsync(id, entity, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id {id}. {@dboEntity}", _entityName, id, dboEntity);

            return dboEntity;
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
    public async Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Logger.LogTrace("{serviceName}.{method} entities: {@entities}", _serviceName, nameof(UpdateAsync), entities);

            Logger.LogDebug("Updating entity in db...");
            var dboEntities = await Repository.UpdateAsync(entities, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated many {name} entities. {@dboEntities}", _entityName, dboEntities);

            return dboEntities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to update many {name} entities: {reason}. {@entities}", _entityName, ex.Message, entities);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,Action{TEntity},CancellationToken)"/>
    /// </summary>
    public async Task<TEntity> UpdateAsync(TId id, Action<TEntity> update, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(update, nameof(update));
            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(UpdateAsync), id);

            Logger.LogDebug("Updating {name} entity with id '{id}' in db...", _entityName, id);
            var dboEntity = await Repository.UpdateAsync(id, update, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Updated {name} entity with id {id}. {@dboEntity}", _entityName, id, dboEntity);

            return dboEntity;
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
    public async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(DeleteAsync), id);

            Logger.LogDebug("Deleting {name} entity with id '{id}' in db...", _entityName, id);
            await Repository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id {id}", _entityName, id);
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
    public async Task DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            Logger.LogTrace("{serviceName}.{method} ids: {@ids}", _serviceName, nameof(DeleteAsync), ids);

            Logger.LogDebug("Updating {name} entities in db...", _entityName);
            await Repository.DeleteAsync(ids, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted many {name} entites with ids: {@ids}", _entityName, ids);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to delete many {name} entities by ids '{@ids}': {reason}", _entityName, ids, ex.Message);
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAndReturnAsync"/>
    /// </summary>
    public async Task<TEntity> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            Logger.LogTrace("{serviceName}.{method} id: {id}", _serviceName, nameof(DeleteAndReturnAsync), id);

            Logger.LogDebug("Deleting {name} entity with id '{id}' in db...", _entityName, id);
            var dboEntity = await Repository.DeleteAndReturnAsync(id, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Deleted {name} entity with id {id}. {@dboEntity}", _entityName, id, dboEntity);

            return dboEntity;
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