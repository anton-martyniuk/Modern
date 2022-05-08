using Modern.Data.Paging;
using Modern.Exceptions;

namespace Modern.Services.DataStore.InMemory.Abstractions.Cache;

/// <summary>
/// The cache definition
/// </summary>
/// <typeparam name="TEntity">The type of entity in the cache</typeparam>
/// <typeparam name="TId">The Entity identifier. Id must be unique</typeparam>
public interface IModernServiceCache<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Adds or updates <paramref name="entity"/> with the given <paramref name="id"/> in the cache.<br />
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">Entity</param>
    ValueTask AddOrUpdateAsync(TId id, TEntity entity);

    /// <summary>
    /// Adds or updates multiple <paramref name="entities"/> in the cache
    /// </summary>
    /// <param name="entities">Set of entities</param>
    ValueTask AddOrUpdateAsync(Dictionary<TId, TEntity> entities);

    /// <summary>
    /// Deletes the entity in the cache by the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    ValueTask DeleteAsync(TId id);

    /// <summary>
    /// Deletes multiple entities in the cache by the given list of <paramref name="ids"/>
    /// </summary>
    /// <param name="ids">List of entity ids</param>
    ValueTask DeleteAsync(List<TId> ids);

    /// <summary>
    /// Deletes all entities in the cache
    /// </summary>
    ValueTask ClearAsync();

    /// <summary>
    /// Returns an entity from the cache with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    /// <exception cref="EntityNotFoundException">Thrown if entity is not found in the cache</exception>
    ValueTask<TEntity> GetByIdAsync(TId id);

    /// <summary>
    /// Tries to return an entity from the cache with the given <paramref name="id"/>; otherwise, <see langword="null"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="id">The entity id</param>
    /// <returns>Entity</returns>
    ValueTask<TEntity?> TryGetByIdAsync(TId id);

    /// <summary>
    /// Tries to return a list of entities from the cache with the given list of <paramref name="ids"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="ids"></param>
    /// <returns>List of entities</returns>
    ValueTask<List<TEntity>> TryGetManyAsync(List<TId> ids);

    /// <summary>
    /// Returns all entities from the cache
    /// IMPORTANT: there can be performance issues when retrieving large amount of entities from the cache
    /// </summary>
    /// <returns>List of entities</returns>
    ValueTask<List<TEntity>> GetAllAsync();

    /// <summary>
    /// Returns the total count of entities in the cache
    /// </summary>
    /// <returns>Count of entities</returns>
    ValueTask<int> CountAsync();

    /// <summary>
    /// Returns the total count of entities in the cache that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>The count of filtered entities in the cache</returns>
    ValueTask<int> CountAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Determines whether the cache contains at least one entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns><see langword="true"/> if at least one entity exists; otherwise, <see langword="false"/></returns>
    ValueTask<bool> ExistsAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns the first entity from the cache that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>Entity or null if not found</returns>
    ValueTask<TEntity?> FirstOrDefaultAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns the single entity from the cache that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <exception cref="InvalidOperationException">Thrown if the data store contains more than one entity that matches the condition</exception>
    /// <returns>Entity or null if not found</returns>
    ValueTask<TEntity?> SingleOrDefaultAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns all entities that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <returns>Enumeration of entities</returns>
    ValueTask<List<TEntity>> WhereAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Returns certain amount of paged entities from the data store that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">The filtering predicate</param>
    /// <param name="pageNumber">Page number. Entities to skip = (pageNumber - 1) * pageSize</param>
    /// <param name="pageSize">The total number of items to select</param>
    /// <returns>A list of entities that match the condition</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    ValueTask<PagedResult<TEntity>> WhereAsync(Func<TEntity, bool> predicate, int pageNumber, int pageSize);

    /// <summary>
    /// Returns <see cref="IEnumerable{TEntity}"/> implementation with data from the cache
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The members of the returned <see cref="IEnumerable{TEntity}"/> instance can throw implementation specific exceptions
    /// </remarks>
    /// <returns>The object typed as <see cref="IEnumerable{TEntity}"/></returns>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    IEnumerable<TEntity> AsEnumerable();
}