using Modern.Cache.DependencyInjection.Definitions.Configuration;
using Modern.Cache.Redis.Configuration;
using StackExchange.Redis.Extensions.Core.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern cache options for registering in DI
/// </summary>
public class ModernRedisCacheOptions
{
    /// <summary>
    /// Collection of modern cache specifications
    /// </summary>
    internal List<ModernCacheSpecification> Caches { get; } = new();

    /// <summary>
    /// Cache settings
    /// </summary>
    public ModernRedisCacheSettings RedisCacheSettings { get; init; } = new();

    /// <summary>
    /// Redis configuration
    /// </summary>
    public RedisConfiguration RedisConfiguration { get; init; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public ModernRedisCacheOptions()
    {
        RedisConfiguration = new RedisConfiguration
        {
            ConnectionString = "localhost"
        };
    }

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