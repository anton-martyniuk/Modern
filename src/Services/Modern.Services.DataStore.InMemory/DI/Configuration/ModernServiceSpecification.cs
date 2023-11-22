using Microsoft.Extensions.DependencyInjection;

namespace Modern.Services.DataStore.InMemory.DI.Configuration;

/// <summary>
/// The modern services specification model
/// </summary>
public class ModernServiceSpecification
{
    /// <summary>
    /// The type of entity returned from the service
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

    /// <summary>
    /// Services lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}