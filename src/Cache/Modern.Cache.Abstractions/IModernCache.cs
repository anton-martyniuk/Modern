using Modern.Exceptions;

namespace Modern.Cache.Abstractions;

/// <summary>
/// The distributed cache definition
/// </summary>
/// <typeparam name="TEntity">The type of entity in the cache</typeparam>
/// <typeparam name="TId">The Entity identifier. Id must be unique</typeparam>
public interface IModernCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Returns an entity from the cache with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    /// <exception cref="EntityNotFoundException">Thrown if entity is not found in the cache</exception>
    Task<TEntity> GetByIdAsync(TId id);

    /// <summary>
    /// Tries to return an entity from the cache with the given <paramref name="id"/>; otherwise, <see langword="null"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    Task<TEntity?> TryGetByIdAsync(TId id);

    /// <summary>
    /// Tries to return a list of entities from the cache with the given list of <paramref name="ids"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<List<TEntity>> TryGetManyAsync(List<TId> ids);

    /// <summary>
    /// Adds or updates <paramref name="entity"/> with the given <paramref name="id"/> in the cache.<br />
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">Entity</param>
    Task AddOrUpdateAsync(TId id, TEntity entity);

    /// <summary>
    /// Adds or updates multiple <paramref name="entities"/> in the cache
    /// </summary>
    /// <param name="entities">Set of entities</param>
    Task AddOrUpdateAsync(Dictionary<TId, TEntity> entities);

    /// <summary>
    /// Deletes the entity in the cache by the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    Task DeleteAsync(TId id);

    /// <summary>
    /// Deletes multiple entities in the cache by the given list of <paramref name="ids"/>
    /// </summary>
    /// <param name="ids">List of entity ids</param>
    Task DeleteAsync(List<TId> ids);
}
