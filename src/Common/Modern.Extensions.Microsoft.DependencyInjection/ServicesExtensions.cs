using Microsoft.Extensions.DependencyInjection;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Modern.Extensions.Microsoft.DependencyInjection;

/// <summary>
/// Modern Builder extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds Modern stuff into DI
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddModern(this IServiceCollection services)
    {
        return new ModernServicesBuilder(services);
    }
}
