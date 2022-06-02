using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Cache.Abstractions;
using Modern.Cache.Abstractions.Configuration;
using Modern.Cache.Redis;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using StackExchange.Redis.Extensions.Core.Configuration;
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
    /// <param name="configureRedis">Redis configure delegate</param>
    /// <param name="configureSettings">Cache settings configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRedisCache(this ModernServicesBuilder builder, Action<ModernCacheOptions> configure,
        Action<RedisConfiguration> configureRedis, Action<ModernCacheSettings> configureSettings)
    {
        // TODO: merge 3 actions into one

        var options = new ModernCacheOptions();
        configure(options);

        foreach (var c in options.Caches)
        {
            var interfaceType = typeof(IModernCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernRedisCache<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        var redisConfiguration = new RedisConfiguration { ConnectionString = "localhost" };
        configureRedis(redisConfiguration);

        builder.Services.Configure(configureSettings);
        builder.Services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
        return builder;
    }
}
