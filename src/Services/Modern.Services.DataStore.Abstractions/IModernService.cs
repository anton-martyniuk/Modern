namespace Modern.Services.DataStore.Abstractions;

/// <summary>
/// Represents an <see cref="IModernCrudService{TEntityDto,TId}"/> and <see cref="IModernQueryService{TEntityDto, TEntityDbo,TId}"/> abstraction
/// with data access through generic repository
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public interface IModernService<TEntityDto, TEntityDbo, TId> :
    IModernCrudService<TEntityDto, TId>,
    IModernQueryService<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
}
