using HotChocolate.Types;
using Modern.Services.DataStore.Abstractions;

namespace Modern.GraphQL.HotChocolate.DataStore;

/// <summary>
/// The modern GraphQL query model
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernGraphQlQuery<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernService<TEntityDto, TEntityDbo, TId> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service">Entity service</param>
    public ModernGraphQlQuery(IModernService<TEntityDto, TEntityDbo, TId> service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns an entity with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>The entity; <see langword="null"/> if not found</returns>
    public virtual async Task<TEntityDto?> GetByIdAsync(TId id)
    {
        var entity = await _service.TryGetByIdAsync(id).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Returns all entities
    /// </summary>
    /// <returns>List of entities</returns>
    public virtual async Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return entities;
    }

    /// <summary>
    /// Returns the total count of entities
    /// </summary>
    /// <returns>Count of entities</returns>
    public virtual async Task<int> GetCountAllAsync()
    {
        var count = await _service.CountAsync().ConfigureAwait(false);
        return count;
    }

    // TODO: create AsQueryable with offset based pagination

    /// <summary>
    /// Performs a GraphQL based query on <see cref="IQueryable{TEntityDbo}"/>
    /// </summary>
    /// <returns>Filtered entities by GraphQL</returns>
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<TEntityDbo> AsQueryable()
    {
        return _service.AsQueryable();
    }
}
