namespace Modern.Repositories.Abstractions;

/// <summary>
/// The generic repository definition for CRUD and Query operations
/// </summary>
/// <typeparam name="TEntity">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of the entity's identifier (mainly primary key)</typeparam>
public interface IModernRepository<TEntity, TId> : IModernCrudRepository<TEntity, TId>, IModernQueryRepository<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
}