namespace Modern.Services.DataStore.InMemory.Abstractions;

/// <summary>
/// Represents an <see cref="IModernCrudInMemoryService{TEntityDto, TId}"/> and <see cref="IModernQueryInMemoryService{TEntityDto, TEntityDbo,TId}"/> abstraction
/// with data access with caching and through generic repository
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public interface IModernInMemoryService<TEntityDto, out TEntityDbo, TId> :
    IModernCrudInMemoryService<TEntityDto, TId>,
    IModernQueryInMemoryService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
}
