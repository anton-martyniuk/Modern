using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Controllers.CQRS.DataStore;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Services.DataStore.Cached extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds controllers into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Controllers configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddCqrsControllers(this ModernServicesBuilder builder, Action<ModernControllersOptions> configure)
    {
        var options = new ModernControllersOptions();
        configure(options);

        builder.Services.AddMvc().AddControllersAsServices();

        foreach (var c in options.Controllers)
        {
            var implementationType = typeof(ModernCqrsController<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            builder.Services.TryAdd(new ServiceDescriptor(implementationType, implementationType, ServiceLifetime.Scoped));
        }

        foreach (var c in options.ConcreteControllers)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.ImplementationType, c.ImplementationType, ServiceLifetime.Scoped));
        }

        return builder;
    }
}
