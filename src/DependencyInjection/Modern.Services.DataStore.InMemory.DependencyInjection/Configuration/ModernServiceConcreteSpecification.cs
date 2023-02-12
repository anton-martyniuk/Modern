using Microsoft.Extensions.DependencyInjection;

namespace Modern.Services.DataStore.InMemory.DependencyInjection.Configuration;

/// <summary>
/// The modern concrete service specification model
/// </summary>
public class ModernServiceConcreteSpecification
{
    /// <summary>
    /// The type of concrete service interface
    /// </summary>
    public Type InterfaceType { get; set; } = default!;

    /// <summary>
    /// The type of concrete service implementation
    /// </summary>
    public Type ImplementationType { get; set; } = default!;
    
    /// <summary>
    /// The type of entity returned from the service
    /// </summary>
    public Type EntityDtoType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;

    /// <summary>
    /// Service lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}