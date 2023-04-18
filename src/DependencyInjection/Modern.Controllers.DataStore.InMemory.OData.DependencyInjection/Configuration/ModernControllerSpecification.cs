namespace Modern.Controllers.DataStore.InMemory.OData.DependencyInjection.Configuration;

/// <summary>
/// The modern controller specification model
/// </summary>
public class ModernControllerSpecification
{
    /// <summary>
    /// The type of request that creates an entity
    /// </summary>
    public Type CreateRequestType { get; set; } = default!;

    /// <summary>
    /// The type of request that updates an entity
    /// </summary>
    public Type UpdateRequestType { get; set; } = default!;

    /// <summary>
    /// The type of entity returned from the controller
    /// </summary>
    public Type EntityDtoType { get; set; } = default!;

    /// <summary>
    /// The type of entity contained in the data store
    /// </summary>
    public Type EntityDboType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;
}