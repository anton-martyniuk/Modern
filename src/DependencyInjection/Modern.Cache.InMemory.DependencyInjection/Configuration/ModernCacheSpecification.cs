using Microsoft.Extensions.DependencyInjection;

namespace Modern.Cache.InMemory.DependencyInjection.Configuration;

/// <summary>
/// The modern cache specification model
/// </summary>
public class ModernCacheSpecification
{
    /// <summary>
    /// The type of entity contained in the cache
    /// </summary>
    public Type EntityType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;

    /// <summary>
    /// Services lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}