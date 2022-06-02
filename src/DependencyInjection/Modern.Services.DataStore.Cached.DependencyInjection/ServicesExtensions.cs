using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Services.DataStore.Abstractions;
using Modern.Services.DataStore.Cached;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Services.DataStore.Cached extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds cached services into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Services configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddCachedServices(this ModernServicesBuilder builder, Action<ModernServicesOptions> configure)
    {
        var options = new ModernServicesOptions();
        configure(options);

        foreach (var c in options.Services)
        {
            var interfaceType = typeof(IModernService<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            var implementationType = typeof(ModernCachedService<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        foreach (var c in options.ConcreteServices)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
