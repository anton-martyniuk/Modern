using System.Linq.Expressions;
using Modern.Exceptions;

namespace Modern.Services.Abstractions
{
    /// <summary>
    /// The generic service definition for querying operations
    /// </summary>
    /// <typeparam name="TEntity">The type of entity contained in the service</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier (mainly primary key)</typeparam>
    public interface IModernQueryService<TEntity, in TId>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Returns an entity from with the given <paramref name="id"/>
        /// </summary>
        /// <param name="id">The entity id</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>Operation result</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
        /// <exception cref="EntityNotFoundException">Thrown if an entity does is not found</exception>
        /// <exception cref="InternalErrorException">If a service internal error occurred</exception>
        Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return an entity with the given <paramref name="id"/>; otherwise, <see langword="null"/>
        /// </summary>
        /// <remarks>
        /// Method does not throw exception if entity is not found
        /// </remarks>
        /// <param name="id">The entity id</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>Operation result</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<TEntity?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all entities.<br/>
        /// IMPORTANT: there can be performance issues when retrieving large amount of entities
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>The list of all entities</returns>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the total count of entities
        /// </summary>
        /// <returns>Count of entities</returns>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the total count of entities that match the given <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for condition</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>Count of entities</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the data store contains at least one entity that matches the given <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for condition</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns><see langword="true"/> if at least one entity exists; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the first entity that matches the given <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for condition</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the single entity that matches the given <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for condition</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all entities that match the given <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for condition</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
        /// <returns>A list of entities that match the condition</returns>
        /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns <see cref="IQueryable{TEntity}"/> implementation
        /// </summary>
        /// <remarks>
        /// IMPORTANT: The members of the returned <see cref="IQueryable{TEntity}"/> instance can throw implementation specific exceptions
        /// </remarks>
        /// <returns>The object typed as <see cref="IQueryable{TEntity}"/></returns>
        /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
        IQueryable<TEntity> AsQueryable();
    }
}