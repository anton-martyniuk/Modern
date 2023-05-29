using Microsoft.Extensions.Caching.Memory;
using Modern.Cache.InMemory.DependencyInjection.Configuration;

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
    /// Memory cache settings
    /// </summary>
    public MemoryCacheEntryOptions CacheSettings { get; init; } = new();

    /// <summary>
    /// Adds cache
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Singleton by default)</param>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity identifier</typeparam>
    public void AddCache<TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
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