using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Modern.Cache.Abstractions;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Exceptions;
using Modern.Services.Abstractions;

namespace Modern.Services;

/// <summary>
/// Represents an <see cref="IModernCrudService{TEntity,TId}"/> and <see cref="IModernQueryService{TEntity,TId}"/> implementation using cache and generic repository
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
/// <typeparam name="TRepository">Type of repository used for the entity</typeparam>
public abstract class ModernService<TEntity, TId, TRepository> : IModernCrudService<TEntity, TId>, IModernQueryService<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntity, TId>, IModernCrudRepository<TEntity, TId>
{
    private readonly string _serviceName = $"{typeof(TEntity).Name}Service";

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly TRepository Repository;

    private readonly IModernCache<TEntity, TId> _cache;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="cache">The cache of entities</param>
    /// <param name="logger">The logger</param>
    protected ModernService(TRepository repository, IModernCache<TEntity, TId> cache, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        _cache = cache;
        Logger = logger;
    }

    /// <summary>
    /// Returns standardizes service exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
    {
        return ex switch
        {
            ArgumentException _ => ex,
            EntityConcurrentUpdateException _ => ex,
            EntityAlreadyExistsException _ => ex,
            EntityNotFoundException _ => ex,
            EntityNotModifiedException _ => ex,
            _ => new RepositoryErrorException(ex.Message, ex)
        };
    }

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
                // TODO: logging
                Logger.LogTrace($"{_serviceName}.{nameof(GetByIdAsync)} id: {{@id}}", id);
            }

            return await Repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // TODO: logging
            Logger.LogError(ex, $"Could not get entity by id: {{@id}} (cause: {ex.Message})", id);
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
                Logger.LogTrace($"{_serviceName}.{nameof(TryGetByIdAsync)} id: {{@id}}", id);
            }

            return await Repository.TryGetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get entity by id: {{@id}} (cause: {ex.Message})", id);
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
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(GetAllAsync)}");
            }

            return await Repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get all entities: {{@id}} (cause: {ex.Message})");
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
                Logger.LogTrace($"{_serviceName}.{nameof(CountAsync)} of all entities");
            }

            return await Repository.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
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

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(CountAsync)}");
            }

            return await Repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get entities count with predicate: {{predicate}} (cause: {ex.Message})", predicate);
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

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(ExistsAsync)}");
            }

            return await Repository.ExistsAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not check entity existence with predicate: {{predicate}} (cause: {ex.Message})", predicate);
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

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(FirstOrDefaultAsync)}");
            }

            return await Repository.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get first entity with predicate: {{predicate}} (cause: {ex.Message})", predicate);
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

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(SingleOrDefaultAsync)}");
            }

            return await Repository.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get single entity with predicate: {{predicate}} (cause: {ex.Message})", predicate);
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

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(WhereAsync)}");
            }

            return await Repository.WhereAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not get entities with predicate: {{predicate}} (cause: {ex.Message})", predicate);
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
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"{_serviceName}.{nameof(AsQueryable)}");
            }

            return Repository.AsQueryable();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
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

            Logger.LogInformation($"{_serviceName}.{nameof(CreateAsync)}: {{@entity}}", entity);

            // TODO: TEntity in service should be DTO or DBO ?!

            // TODO: accept TEntityDto and TEntityDbo in Service template arguments

            // TODO: Service with and without cache

            Logger.LogInformation("Creating entity in db...");
            var dboEntity = await Repository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            Logger.LogInformation("Created. {@dboEntity}", dboEntity);

            //var dboEntity = MapDtoToDbo(entity);

            //Logger.LogInformation("Creating entity in db...");
            //await Repository.CreateAsync(dboEntity, cancellationToken).ConfigureAwait(false);
            //Logger.LogInformation($"Created. Id: {dboEntity.Id}");

            //Logger.LogDebug("Adding entity to cache...");
            //var entityDto = MapDtoToDbo(dboEntity);
            //_cache.AddOrUpdate(entityDto.Id, entityDto);
            //Logger.LogDebug("Added.");

            return dboEntity;
        }
        catch (Exception e)
        {
            Logger.LogError($"Unable to create a new entity. {e.Message}", e, entity);
            throw CreateProperException(e);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.CreateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public Task<List<TEntity>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,TEntity,CancellationToken)"/>
    /// </summary>
    public Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,Action{TEntity},CancellationToken)"/>
    /// </summary>
    public Task<TEntity> UpdateAsync(TId id, Action<TEntity> update, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(List{TId},CancellationToken)"/>
    /// </summary>
    public Task DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAndReturnAsync"/>
    /// </summary>
    public Task<TEntity> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}