using Modern.Services.Abstractions.Crud;
using Modern.Services.Abstractions.Query;

namespace Modern.Services.Abstractions;

/// <summary>
/// Represents an <see cref="IModernCrudService{TEntityDto, TEntityDbo,TId}"/> and <see cref="IModernQueryService{TEntityDto, TEntityDbo,TId}"/> abstraction
/// with data access through generic repository
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public interface IModernEntityService<TEntityDto, TEntityDbo, TId> :
    IModernCrudService<TEntityDto, TEntityDbo, TId>,
    IModernQueryService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
}
