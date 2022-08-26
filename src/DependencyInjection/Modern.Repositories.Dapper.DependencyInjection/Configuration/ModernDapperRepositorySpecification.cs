using Microsoft.Extensions.DependencyInjection;

namespace Modern.Repositories.Dapper.DependencyInjection.Configuration;

/// <summary>
/// The modern repository specification model
/// </summary>
public class ModernDapperRepositorySpecification
{
    /// <summary>
    /// The type of entity mapping
    /// </summary>
    public Type EntityMappingType { get; set; } = default!;

    /// <summary>
    /// The type of entity contained in the data store
    /// </summary>
    public Type EntityType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;

    /// <summary>
    /// Repository lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}