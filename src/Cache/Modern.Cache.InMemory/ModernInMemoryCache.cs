using System.Collections.Concurrent;
using Modern.Cache.Abstractions;
using Modern.Exceptions;

namespace Modern.Cache.InMemory;

/// <summary>
/// The <see cref="IModernCache{TEntity,TId}"/> implementation using <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
/// <typeparam name="TId">Type of entity identifier</typeparam>
public class ModernInMemoryCache<TEntity, TId> : IModernCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly ConcurrentDictionary<TId, TEntity> _cacheById;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public ModernInMemoryCache()
    {
        _cacheById = new ConcurrentDictionary<TId, TEntity>(EqualityComparer<TId>.Default);
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="equalityComparer">Equality comparer for the <typeparamref name="TId"/></param>
    public ModernInMemoryCache(IEqualityComparer<TId> equalityComparer)
    {
        _cacheById = new ConcurrentDictionary<TId, TEntity>(equalityComparer);
    }

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.AddOrUpdate"/>
    /// </summary>
    public void AddOrUpdate(TId id, TEntity entity) => _cacheById.AddOrUpdate(id, entity, (_, _) => entity);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Delete"/>
    /// </summary>
    public void Delete(TId id) => _cacheById.TryRemove(id, out _);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Clear"/>
    /// </summary>
    public void Clear() => _cacheById.Clear();

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.GetById"/>
    /// </summary>
    public TEntity GetById(TId id) => _cacheById.TryGetValue(id, out var entity) ? entity : throw new EntityNotFoundException($"Entity with id '{id}' not found");

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.TryGetById"/>
    /// </summary>
    public TEntity? TryGetById(TId id) => _cacheById.TryGetValue(id, out var entity) ? entity : null;

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.GetAll"/>
    /// </summary>
    public IEnumerable<TEntity> GetAll() => _cacheById.Values.AsEnumerable();

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Count()"/>
    /// </summary>
    public int Count() => _cacheById.Values.Count;

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Count(Func{TEntity, bool})"/>
    /// </summary>
    public int Count(Func<TEntity, bool> predicate) => _cacheById.Values.Count(predicate);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Exists"/>
    /// </summary>
    public bool Exists(Func<TEntity, bool> predicate) => _cacheById.Values.Any(predicate);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.FirstOrDefault"/>
    /// </summary>
    public TEntity? FirstOrDefault(Func<TEntity, bool> predicate) => _cacheById.Values.FirstOrDefault(predicate);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.SingleOrDefault"/>
    /// </summary>
    public TEntity? SingleOrDefault(Func<TEntity, bool> predicate) => _cacheById.Values.SingleOrDefault(predicate);

    /// <summary>
    /// <inheritdoc cref="IModernCache{TEntity,TId}.Where"/>
    /// </summary>
    public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate) => _cacheById.Values.Where(predicate);
}