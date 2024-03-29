﻿using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Services.DataStore.InMemory;
using Modern.Services.DataStore.InMemory.Abstractions;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;
using Modern.Services.DataStore.InMemory.Cache;
using Modern.Services.DataStore.InMemory.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Services.DataStore.InMemory extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds InMemory services into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Services configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddInMemoryServices(this ModernServicesBuilder builder, Action<ModernServicesOptions> configure)
    {
        var options = new ModernServicesOptions();
        configure(options);
        
        if (options.ServiceConfiguration is not null)
        {
            builder.Services.Configure<ModernInMemoryServiceConfiguration>(x =>
            {
                x.AddToCacheWhenEntityCreated = options.ServiceConfiguration.AddToCacheWhenEntityCreated;
                x.AddOrUpdateInCacheWhenEntityIsUpdated = options.ServiceConfiguration.AddOrUpdateInCacheWhenEntityIsUpdated;
            });   
        }

        foreach (var c in options.Services)
        {
            var interfaceType = typeof(IModernInMemoryService<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            var implementationType = typeof(ModernInMemoryService<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);

            var cacheInterfaceType = typeof(IModernServiceCache<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
            var cacheImplementationType = typeof(ModernInMemoryServiceCache<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(cacheInterfaceType, cacheImplementationType, ServiceLifetime.Singleton));
        }

        foreach (var c in options.ConcreteServices)
        {
            var cacheInterfaceType = typeof(IModernServiceCache<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
            var cacheImplementationType = typeof(ModernInMemoryServiceCache<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
            
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(cacheInterfaceType, cacheImplementationType, ServiceLifetime.Singleton));
        }

        return builder;
    }
}
