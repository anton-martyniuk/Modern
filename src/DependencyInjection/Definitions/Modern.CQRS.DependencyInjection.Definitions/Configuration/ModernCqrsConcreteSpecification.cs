using Microsoft.Extensions.DependencyInjection;

namespace Modern.CQRS.DependencyInjection.Definitions.Configuration;

/// <summary>
/// The modern concrete CQRS specification model
/// </summary>
public class ModernCqrsConcreteSpecification
{
    /// <summary>
    /// The type of concrete CQRS interface
    /// </summary>
    public Type InterfaceType { get; set; } = default!;

    /// <summary>
    /// The type of concrete CQRS implementation
    /// </summary>
    public Type ImplementationType { get; set; } = default!;

    /// <summary>
    /// Service lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}