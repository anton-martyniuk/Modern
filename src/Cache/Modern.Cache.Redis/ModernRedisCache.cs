using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;
using Modern.Cache.Abstractions;
using Modern.Cache.Redis.Configuration;
using Modern.Exceptions;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Modern.Cache.Redis;

/// <summary>
/// The <see cref="IModernCache{TEntity,TId}"/> implementation using Redis
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
/// <typeparam name="TId">Type of entity identifier</typeparam>
public class ModernRedisCache<TEntity, TId> : IModernCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly IRedisClient _redisClient;
    private readonly ModernRedisCacheSettings _cacheSettings;

    private readonly string _redisKeyPrefix = $"modern_cache_{typeof(TEntity).Name}".ToLower();

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="redisClient">Redis client (wrapper around Redis Core)</param>
    /// <param name="cacheSettings">Cache settings</param>
    public ModernRedisCache(IRedisClient redisClient, IOptions<ModernRedisCacheSettings> cacheSettings)
    {
        _redisClient = redisClient;
        _cacheSettings = cacheSettings.Value;
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.GetByIdAsync"/>
    /// </summary>
    public async ValueTask<TEntity> GetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);

        var entity = _cacheSettings.ExpiresIn is null
            ? await _redisClient.GetDefaultDatabase().GetAsync<TEntity>(key).ConfigureAwait(false)
            : await _redisClient.GetDefaultDatabase().GetAsync<TEntity>(key, _cacheSettings.ExpiresIn.Value).ConfigureAwait(false);
        
        if (entity is null)
        {
            throw new EntityNotFoundException($"Entity with id '{id}' not found");
        }

        return entity;
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.TryGetByIdAsync"/>
    /// </summary>
    public async ValueTask<TEntity?> TryGetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);
        
        var entity = _cacheSettings.ExpiresIn is null
            ? await _redisClient.GetDefaultDatabase().GetAsync<TEntity>(key).ConfigureAwait(false)
            : await _redisClient.GetDefaultDatabase().GetAsync<TEntity>(key, _cacheSettings.ExpiresIn.Value).ConfigureAwait(false);

        return entity;
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.TryGetManyAsync"/>
    /// </summary>
    public async ValueTask<List<TEntity>> TryGetManyAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        var keys = ids.Select(x => GetKey(x)).ToHashSet();
        var entities = await _redisClient.GetDefaultDatabase().GetAllAsync<TEntity>(keys).ConfigureAwait(false);
        return entities.Values.Where(x => x is not null).ToList()!;
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.AddOrUpdateAsync(TId,TEntity)"/>
    /// </summary>
    public async ValueTask AddOrUpdateAsync(TId id, TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);

        if (_cacheSettings.ExpiresIn is null)
        {
            await _redisClient.GetDefaultDatabase().AddAsync(key, entity).ConfigureAwait(false);
            return;
        }

        await _redisClient.GetDefaultDatabase().AddAsync(key, entity, _cacheSettings.ExpiresIn.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.AddOrUpdateAsync(Dictionary{TId,TEntity})"/>
    /// </summary>
    public async ValueTask AddOrUpdateAsync(Dictionary<TId, TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        Guard.Against.NegativeOrZero(entities.Count, nameof(entities));

        var items = entities.Select(x => Tuple.Create(GetKey(x.Key), x.Value)).ToArray();

        if (_cacheSettings.ExpiresIn is null)
        {
            await _redisClient.GetDefaultDatabase().AddAllAsync(items).ConfigureAwait(false);
            return;
        }

        await _redisClient.GetDefaultDatabase().AddAllAsync(items, _cacheSettings.ExpiresIn.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.DeleteAsync(TId)"/>
    /// </summary>
    public async ValueTask DeleteAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var key = GetKey(id);
        await _redisClient.GetDefaultDatabase().RemoveAsync(key).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.DeleteAsync(List{TId})"/>
    /// </summary>
    public async ValueTask DeleteAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        var keys = ids.Select(x => GetKey(x)).ToArray();
        await _redisClient.GetDefaultDatabase().RemoveAllAsync(keys).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns key name to access entity in the cache
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>Key name</returns>
    private string GetKey(TId id) => $"{_redisKeyPrefix}_{id}";
}
