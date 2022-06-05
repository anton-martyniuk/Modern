using Modern.Cache.Abstractions.Configuration;
using Modern.Cache.DependencyInjection.Definitions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern cache options for registering in DI
/// </summary>
public class ModernCacheOptions
{
    /// <summary>
    /// Collection of modern cache specifications
    /// </summary>
    internal List<ModernCacheSpecification> Caches { get; } = new();

    /// <summary>
    /// Cache settings
    /// </summary>
    public ModernCacheSettings CacheSettings { get; init; } = new();

    /// <summary>
    /// Adds cache
    /// </summary>
    /// <param name="lifetime">Cache lifetime in DI</param>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity identifier</typeparam>
    public void AddCache<TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernCacheSpecification
        {
            EntityType = typeof(TEntity),
            EntityIdType = typeof(TId),
            Lifetime = lifetime
        };

        Caches.Add(configuration);
    }
}