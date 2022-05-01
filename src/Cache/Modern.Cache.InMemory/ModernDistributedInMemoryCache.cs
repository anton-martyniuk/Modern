using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Modern.Cache.Abstractions;
using Modern.Cache.Abstractions.Configuration;
using Modern.Exceptions;

namespace Modern.Cache.InMemory;

/// <summary>
/// The <see cref="IModernDistributedCache{TEntity,TId}"/> implementation using in-memory cache
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
/// <typeparam name="TId">Type of entity identifier</typeparam>
public class ModernDistributedInMemoryCache<TEntity, TId> : IModernDistributedCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly IMemoryCache _cache;
    private readonly ModernCacheSettings _cacheSettings;

    private readonly string _redisKeyPrefix = $"cache_{nameof(TEntity)}".ToLower();

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="cache">In-memory cache</param>
    /// <param name="cacheSettings">Cache settings</param>
    public ModernDistributedInMemoryCache(IMemoryCache cache, IOptions<ModernCacheSettings> cacheSettings)
    {
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.GetByIdAsync"/>
    /// </summary>
    public Task<TEntity> GetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);
        var entity = _cache.Get<TEntity>(key);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Entity with id '{id}' not found");
        }

        return Task.FromResult(entity);
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.TryGetByIdAsync"/>
    /// </summary>
    public Task<TEntity?> TryGetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);
        return Task.FromResult(_cache.Get<TEntity?>(key));
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.TryGetManyAsync"/>
    /// </summary>
    public Task<List<TEntity>> TryGetManyAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        var keys = ids.Select(x => GetKey(x)).ToArray();
        var entities = keys.Select(x => _cache.Get<TEntity>(x)).Where(x => x is not null);
        return Task.FromResult(entities.ToList());
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.AddOrUpdateAsync(TId,TEntity)"/>
    /// </summary>
    public Task AddOrUpdateAsync(TId id, TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);

        if (_cacheSettings.ExpiresIn is null)
        {
            _cache.Set(key, entity);
            return Task.CompletedTask;
        }

        _cache.Set(key, entity, _cacheSettings.ExpiresIn.Value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.AddOrUpdateAsync(Dictionary{TId,TEntity})"/>
    /// </summary>
    public Task AddOrUpdateAsync(Dictionary<TId, TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        Guard.Against.NegativeOrZero(entities.Count, nameof(entities));

        foreach (var (id, value) in entities)
        {
            var key = GetKey(id);

            if (_cacheSettings.ExpiresIn is null)
            {
                _cache.Set(key, value);
            }
            else
            {
                _cache.Set(key, value, _cacheSettings.ExpiresIn.Value);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.DeleteAsync(TId)"/>
    /// </summary>
    public Task DeleteAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);
        _cache.Remove(key);

        return Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernDistributedCache{TEntity,TId}.DeleteAsync(List{TId})"/>
    /// </summary>
    public Task DeleteAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        foreach (var key in ids.Select(x => GetKey(x)))
        {
            _cache.Remove(key);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns key name to access entity in the cache
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>Key name</returns>
    private string GetKey(TId id) => $"{_redisKeyPrefix}_{id}";
}
