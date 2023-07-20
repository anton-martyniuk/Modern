using Modern.Controllers.DataStore.DependencyInjection.Features;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Controllers.DataStore extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds controllers into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Controllers configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddControllers(this ModernServicesBuilder builder, Action<ModernControllersOptions> configure)
    {
        var options = new ModernControllersOptions();
        configure(options);

        builder.Services
            .AddMvc(o => o.Conventions.Add(new GenericControllerRouteConvention(options.Controllers)))
            .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider(options)));

        return builder;
    }
}
