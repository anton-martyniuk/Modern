namespace Modern.Controllers.DependencyInjection.Definitions.Configuration;

/// <summary>
/// The modern controller specification model
/// </summary>
public class ModernControllerSpecification
{
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