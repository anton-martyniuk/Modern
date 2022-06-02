using Microsoft.Extensions.DependencyInjection;

namespace Modern.Repositories.EFCore.DependencyInjection.Configuration;

/// <summary>
/// The modern repository specification model
/// </summary>
public class ModernEfCoreRepositorySpecification
{
    /// <summary>
    /// The type of EF Core DbContext
    /// </summary>
    public Type DbContextType { get; set; } = default!;

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