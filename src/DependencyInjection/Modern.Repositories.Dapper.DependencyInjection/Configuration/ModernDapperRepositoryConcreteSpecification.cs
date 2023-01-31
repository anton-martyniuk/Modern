using Microsoft.Extensions.DependencyInjection;

namespace Modern.Repositories.Dapper.DependencyInjection.Configuration;

/// <summary>
/// The modern concrete repository specification model
/// </summary>
public class ModernDapperRepositoryConcreteSpecification
{
    /// <summary>
    /// The type of entity mapping
    /// </summary>
    public Type EntityMappingType { get; set; } = default!;
    
    /// <summary>
    /// The type of concrete repository interface
    /// </summary>
    public Type InterfaceType { get; set; } = default!;

    /// <summary>
    /// The type of concrete repository implementation
    /// </summary>
    public Type ImplementationType { get; set; } = default!;

    /// <summary>
    /// Repository lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}