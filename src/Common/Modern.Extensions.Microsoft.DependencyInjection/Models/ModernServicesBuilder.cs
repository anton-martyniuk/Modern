using Microsoft.Extensions.DependencyInjection;

namespace Modern.Extensions.Microsoft.DependencyInjection.Models;

/// <summary>
/// Represents a building block of Modern stuff in DI
/// </summary>
public class ModernServicesBuilder
{
    /// <summary>
    /// IServiceCollection
    /// </summary>
    public IServiceCollection Services { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    public ModernServicesBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
