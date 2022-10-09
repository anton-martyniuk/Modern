using Modern.Controllers.DataStore.OData.DependencyInjection.Features;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Services.DataStore.Cached extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds OData controllers into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Controllers configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddODataControllers(this ModernServicesBuilder builder, Action<ModernODataControllersOptions> configure)
    {
        var options = new ModernODataControllersOptions();
        configure(options);

        builder.Services
            .AddMvc(o => o.Conventions.Add(new GenericControllerRouteConvention()))
            .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider(options)));

        return builder;
    }
}
