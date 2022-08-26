using System.Linq.Expressions;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions.Infrastracture;

namespace Modern.Repositories.Abstractions;

/// <summary>
/// The generic repository definition for querying operations
/// </summary>
/// <typeparam name="TEntity">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of the entity's identifier (mainly primary key)</typeparam>
public interface IModernQueryRepository<TEntity, in TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Returns an entity from the data store with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="EntityNotFoundException">Thrown if an entity does is not found in the data store</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Found entity</returns>
    Task<TEntity> GetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to return an entity from the data store with the given <paramref name="id"/>; otherwise, <see langword="null"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="id">The entity id</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Found entity</returns>
    Task<TEntity?> TryGetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities.<br/>
    /// IMPORTANT: there can be performance issues when retrieving large amount of entities from the data store
    /// </summary>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>The list of all entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities in the data store
    /// </summary>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Count of entities</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities in the data store that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Count of entities</returns>
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the data store contains at least one entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns><see langword="true"/> if at least one entity exists; otherwise, <see langword="false"/></returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity from the data store that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the single entity from the data store that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if the data store contains more than one entity that matches the condition</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities from the data store that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>A list of entities that match the condition</returns>
    Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns certain amount of paged entities from the data store that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="pageNumber">Page number. Entities to skip = (pageNumber - 1) * pageSize</param>
    /// <param name="pageSize">The total number of items to select</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>A list of entities that match the condition</returns>
    Task<PagedResult<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see cref="IQueryable{TEntity}"/> implementation
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The members of the returned <see cref="IQueryable{TEntity}"/> instance can throw implementation specific exceptions
    /// </remarks>
    /// <exception cref="RepositoryErrorException">Thrown if an error occurred while retrieving entities from the data store</exception>
    /// <returns>The object typed as <see cref="IQueryable{TEntity}"/></returns>
    IQueryable<TEntity> AsQueryable();
}