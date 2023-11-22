using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Cache.Abstractions;
using Modern.Cache.Redis;
using Modern.Cache.Redis.Configuration;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using StackExchange.Redis.Extensions.Newtonsoft;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Cache.Redis extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds Redis cache into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Cache configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRedisCache(this ModernServicesBuilder builder, Action<ModernRedisCacheOptions> configure)
    {
        var options = new ModernRedisCacheOptions();
        configure(options);

        foreach (var c in options.Caches)
        {
            var interfaceType = typeof(IModernCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernRedisCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        builder.Services.Configure<ModernRedisCacheSettings>(x => x.ExpiresIn = options.RedisCacheSettings.ExpiresIn);
        builder.Services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(options.RedisConfiguration);
        return builder;
    }
}
