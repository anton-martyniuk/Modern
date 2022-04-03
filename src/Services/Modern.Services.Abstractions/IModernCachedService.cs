using Modern.Services.Abstractions.Crud;
using Modern.Services.Abstractions.Query;

namespace Modern.Services.Abstractions;

/// <summary>
/// Represents an <see cref="IModernCrudService{TEntityDto, TEntityDbo, TId}"/> and <see cref="IModernQueryCachedService{TEntityDto, TEntityDbo,TId}"/> abstraction
/// with data access with caching and through generic repository
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public interface IModernCachedService<TEntityDto, out TEntityDbo, TId> :
    IModernCrudCachedService<TEntityDto, TId>,
    IModernQueryCachedService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
}
