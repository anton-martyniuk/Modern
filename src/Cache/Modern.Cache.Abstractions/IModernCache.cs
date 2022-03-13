using Modern.Exceptions;

namespace Modern.Cache.Abstractions;

/// <summary>
/// The cache definition
/// </summary>
/// <typeparam name="TEntity">The type of entity in the cache</typeparam>
/// <typeparam name="TId">The Entity identifier. Id must be unique</typeparam>
public interface IModernCache<TEntity, in TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    // TODO: make async?

    // TODO: add guard clauses?

    // TODO: add, update, delete many

    /// <summary>
    /// Adds or updates <paramref name="entity"/> with the given <paramref name="id"/> in the cache.<br />
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">Entity</param>
    void AddOrUpdate(TId id, TEntity entity);

    /// <summary>
    /// Deletes the entity in the cache by the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    void Delete(TId id);

    /// <summary>
    /// Deletes all entities in the cache
    /// </summary>
    void Clear();

    /// <summary>
    /// Returns an entity from the cache with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    /// <exception cref="EntityNotFoundException">Thrown if entity is not found in the cache</exception>
    TEntity GetById(TId id);

    /// <summary>
    /// Tries to return an entity from the cache with the given <paramref name="id"/>; otherwise, <see langword="null"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    TEntity? TryGetById(TId id);

    /// <summary>
    /// Returns all entities from the cache.<br/>
    /// IMPORTANT: there can be performance issues when retrieving large amount of entities from the cache
    /// </summary>
    /// <returns>List of entities</returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Returns the total count of entities in the cache
    /// </summary>
    /// <returns>Count of entities</returns>
    int Count();

    /// <summary>
    /// Returns the total count of entities in the cache that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>The count of filtered entities in the cache</returns>
    int Count(Func<TEntity, bool> predicate);

    /// <summary>
    /// Determines whether the cache contains at least one entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns><see langword="true"/> if at least one entity exists; otherwise, <see langword="false"/></returns>
    bool Exists(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns the first entity from the cache that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>Entity or null if not found</returns>
    TEntity? FirstOrDefault(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns the single entity from the cache that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <exception cref="InvalidOperationException">Thrown if the data store contains more than one entity that matches the condition</exception>
    /// <returns>Entity or null if not found</returns>
    TEntity? SingleOrDefault(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns all entities that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>Enumeration of entities</returns>
    IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
}