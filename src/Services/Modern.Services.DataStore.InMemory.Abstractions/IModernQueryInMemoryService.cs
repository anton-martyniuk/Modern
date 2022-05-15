using Modern.Data.Paging;
using Modern.Exceptions;

namespace Modern.Services.DataStore.InMemory.Abstractions;

/// <summary>
/// The generic service definition for cached querying operations
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of the entity's identifier (mainly primary key)</typeparam>
public interface IModernQueryInMemoryService<TEntityDto, out TEntityDbo, in TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Returns an entity with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="EntityNotFoundException">Thrown if an entity does is not found</exception>
    /// <exception cref="InternalErrorException">If a service internal error occurred</exception>
    /// <returns>The entity</returns>
    Task<TEntityDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to return an entity with the given <paramref name="id"/>; otherwise, <see langword="null"/>
    /// </summary>
    /// <remarks>
    /// Method does not throw exception if entity is not found
    /// </remarks>
    /// <param name="id">The entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>The entity</returns>
    Task<TEntityDto?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities.<br/>
    /// IMPORTANT: there can be performance issues when retrieving large amount of entities
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>The list of all entities</returns>
    Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities
    /// </summary>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>Count of entities</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>Count of entities</returns>
    Task<int> CountAsync(Func<TEntityDto, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the data store contains at least one entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns><see langword="true"/> if at least one entity exists; otherwise, <see langword="false"/></returns>
    Task<bool> ExistsAsync(Func<TEntityDto, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
    Task<TEntityDto?> FirstOrDefaultAsync(Func<TEntityDto, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the single entity that matches the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>Entity that matches the given <paramref name="predicate"/> or <see langword="null"/> if entity not found</returns>
    Task<TEntityDto?> SingleOrDefaultAsync(Func<TEntityDto, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>A list of entities that match the condition</returns>
    Task<List<TEntityDto>> WhereAsync(Func<TEntityDto, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns certain amount of paged entities from the data store that match the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">A function to test each element for condition</param>
    /// <param name="pageNumber">Page number. Entities to skip = (pageNumber - 1) * pageSize</param>
    /// <param name="pageSize">The total number of items to select</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>A list of entities that match the condition</returns>
    Task<PagedResult<TEntityDto>> WhereAsync(Func<TEntityDto, bool> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see cref="IEnumerable{TEntityDto}"/> implementation with data from the cache
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The members of the returned <see cref="IEnumerable{TEntityDto}"/> instance can throw implementation specific exceptions
    /// </remarks>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>The object typed as <see cref="IEnumerable{TEntityDto}"/></returns>
    IEnumerable<TEntityDto> AsEnumerable();

    /// <summary>
    /// Returns <see cref="IQueryable{TEntityDto}"/> implementation with data from the data store
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The members of the returned <see cref="IQueryable{TEntityDto}"/> instance can throw implementation specific exceptions
    /// </remarks>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
    /// <returns>The object typed as <see cref="IQueryable{TEntityDto}"/></returns>
    IQueryable<TEntityDbo> AsQueryable();
}