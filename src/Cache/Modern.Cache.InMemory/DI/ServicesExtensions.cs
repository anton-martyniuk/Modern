using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Cache.Abstractions;
using Modern.Cache.InMemory;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Cache.InMemory extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds in memory cache into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Cache configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddInMemoryCache(this ModernServicesBuilder builder, Action<ModernCacheOptions> configure)
    {
        var options = new ModernCacheOptions();
        configure(options);

        foreach (var c in options.Caches)
        {
            var interfaceType = typeof(IModernCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernInMemoryCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        builder.Services.Configure<MemoryCacheEntryOptions>(x =>
        {
            x.Priority = options.CacheSettings.Priority;
            x.Size = options.CacheSettings.Size;
            x.AbsoluteExpiration = options.CacheSettings.AbsoluteExpiration;
            x.AbsoluteExpirationRelativeToNow = options.CacheSettings.AbsoluteExpirationRelativeToNow;
            x.SlidingExpiration = options.CacheSettings.SlidingExpiration;
        });

        builder.Services.AddMemoryCache();
        return builder;
    }
}
