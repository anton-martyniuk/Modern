using Modern.Exceptions;

namespace Modern.Services.DataStore.InMemory.Abstractions;

/// <summary>
/// The generic service definition for CRUD cached operations
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TId">The type of the entity's identifier (mainly primary key)</typeparam>
public interface IModernCrudInMemoryService<TEntityDto, TId>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Creates the new entity
    /// </summary>
    /// <param name="entity">The entity to add to the data store</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided entity is null</exception>
    /// <exception cref="EntityAlreadyExistsException">Thrown if an entity already exists in the data store</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entity in the data store</exception>
    /// <returns>Updated entity by the data store (primary key, for example)</returns>
    Task<TEntityDto> CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a list of new entities in the data store
    /// </summary>
    /// <param name="entities">The list of entities to add to the data store</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>A list of updated entities by the data store (primary key, for example)</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided list of entities is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entities in the data store</exception>
    Task<List<TEntityDto>> CreateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the entity in the data store with the given <paramref name="id"/>
    /// <remarks>
    /// Method should retrieve and update the entity within the single transaction.
    /// </remarks>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">The entity which should be updated in the data store</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Updated entity</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided id or entity is null</exception>
    /// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
    /// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entity in the data store</exception>
    Task<TEntityDto> UpdateAsync(TId id, TEntityDto entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the list of entities in the data store with the given list of <paramref name="entities"/>.<br/>
    /// If all or some of entities were not found in the data store - no exception is thrown
    /// <remarks>
    /// Method should retrieve and update the entity within the single transaction.
    /// </remarks>
    /// </summary>
    /// <param name="entities">The list of entities which should be updated in the data store</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>List of updated entities</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
    /// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entities in the data store</exception>
    Task<List<TEntityDto>> UpdateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the entity in the data store with the given <paramref name="id"/>
    /// <remarks>
    /// Method should retrieve and update the entity within the single transaction.<br/>
    /// Method should ignore changing "Id" field outside of the repository (in <paramref name="update"/> action).
    /// </remarks>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="update">The entity update action</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Updated entity</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided id or entity is null</exception>
    /// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
    /// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entity in the data store</exception>
    Task<TEntityDto> UpdateAsync(TId id, Action<TEntityDto> update, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the entity in the data store with the given <paramref name="id"/>.<br/>
    /// This method does NOT query the entity from the data store before deletion
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entity in the data store</exception>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the list of entities in the data store with the given list of <paramref name="ids"/>.<br/>
    /// This method does NOT query the entities from the data store before deletion.<br/>
    /// If all or some of entities were not found in the data store - no exception is thrown
    /// </summary>
    /// <param name="ids">The list of entity ids</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entities in the data store</exception>
    Task DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes and returns an entity in the data store with the given <paramref name="id"/><br/>
    /// This method queries the entity from the data store before deletion
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Deleted entity</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
    /// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
    /// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entity in the data store</exception>
    Task<TEntityDto> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default);
}
