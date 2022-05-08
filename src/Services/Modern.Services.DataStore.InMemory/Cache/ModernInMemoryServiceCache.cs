using System.Collections.Concurrent;
using Ardalis.GuardClauses;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;

namespace Modern.Services.DataStore.InMemory.Cache;

/// <summary>
/// The <see cref="IModernServiceCache{TEntity,TId}"/> implementation using <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
/// <typeparam name="TId">Type of entity identifier</typeparam>
public class ModernInMemoryServiceCache<TEntity, TId> : IModernServiceCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly ConcurrentDictionary<TId, TEntity> _dictionary;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public ModernInMemoryServiceCache()
    {
        _dictionary = new ConcurrentDictionary<TId, TEntity>(EqualityComparer<TId>.Default);
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="equalityComparer">Equality comparer for the <typeparamref name="TId"/></param>
    public ModernInMemoryServiceCache(IEqualityComparer<TId> equalityComparer)
    {
        _dictionary = new ConcurrentDictionary<TId, TEntity>(equalityComparer);
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.AddOrUpdateAsync(TId,TEntity)"/>
    /// </summary>
    public ValueTask AddOrUpdateAsync(TId id, TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _dictionary.AddOrUpdate(id, entity, (_, _) => entity);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.AddOrUpdateAsync(Dictionary{TId, TEntity})"/>
    /// </summary>
    public ValueTask AddOrUpdateAsync(Dictionary<TId, TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        Guard.Against.NegativeOrZero(entities.Count, nameof(entities));

        foreach (var (key, value) in entities)
        {
            _dictionary.AddOrUpdate(key, value, (_, _) => value);
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.DeleteAsync(TId)"/>
    /// </summary>
    public ValueTask DeleteAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        _dictionary.TryRemove(id, out _);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.DeleteAsync(List{TId})"/>
    /// </summary>
    public ValueTask DeleteAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        foreach (var id in ids)
        {
            _dictionary.TryRemove(id, out _);
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.ClearAsync"/>
    /// </summary>
    public ValueTask ClearAsync()
    {
        _dictionary.Clear();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.GetByIdAsync"/>
    /// </summary>
    public ValueTask<TEntity> GetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return ValueTask.FromResult(_dictionary.TryGetValue(id, out var entity)
            ? entity
            : throw new EntityNotFoundException($"Entity with id '{id}' not found"));
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.TryGetByIdAsync"/>
    /// </summary>
    public ValueTask<TEntity?> TryGetByIdAsync(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return ValueTask.FromResult(_dictionary.TryGetValue(id, out var entity) ? entity : null);
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.TryGetManyAsync"/>
    /// </summary>
    public ValueTask<List<TEntity>> TryGetManyAsync(List<TId> ids)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));
        Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

        return ValueTask.FromResult(ids.Where(x => _dictionary.ContainsKey(x)).Select(k => _dictionary[k]).ToList());
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.GetAllAsync"/>
    /// </summary>
    public ValueTask<List<TEntity>> GetAllAsync() => ValueTask.FromResult(_dictionary.Values.ToList());

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.CountAsync()"/>
    /// </summary>
    public ValueTask<int> CountAsync() => ValueTask.FromResult(_dictionary.Values.Count);

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.CountAsync(Func{TEntity, bool})"/>
    /// </summary>
    public ValueTask<int> CountAsync(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return ValueTask.FromResult(_dictionary.Values.Count(predicate));
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.ExistsAsync"/>
    /// </summary>
    public ValueTask<bool> ExistsAsync(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return ValueTask.FromResult(_dictionary.Values.Any(predicate));
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.FirstOrDefaultAsync"/>
    /// </summary>
    public ValueTask<TEntity?> FirstOrDefaultAsync(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return ValueTask.FromResult(_dictionary.Values.FirstOrDefault(predicate));
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.SingleOrDefaultAsync"/>
    /// </summary>
    public ValueTask<TEntity?> SingleOrDefaultAsync(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return ValueTask.FromResult(_dictionary.Values.SingleOrDefault(predicate));
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.WhereAsync(Func{TEntity,bool})"/>
    /// </summary>
    public ValueTask<List<TEntity>> WhereAsync(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return ValueTask.FromResult(_dictionary.Values.Where(predicate).ToList());
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.WhereAsync(Func{TEntity,bool}, int, int)"/>
    /// </summary>
    public ValueTask<PagedResult<TEntity>> WhereAsync(Func<TEntity, bool> predicate, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
        Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

        var pagedResult = new PagedResult<TEntity>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = _dictionary.Values.Count(predicate),
            Items = _dictionary.Values.Where(predicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        };
        return ValueTask.FromResult(pagedResult);
    }

    /// <summary>
    /// <inheritdoc cref="IModernServiceCache{TEntity,TId}.AsEnumerable"/>
    /// </summary>
    public IEnumerable<TEntity> AsEnumerable() => _dictionary.Values.AsEnumerable();
}