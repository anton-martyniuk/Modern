using Modern.Services.DataStore.Cached.Configuration;
using Modern.Services.DataStore.Cached.DependencyInjection.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern services options for registering in DI
/// </summary>
public class ModernServicesOptions
{
    /// <summary>
    /// The modern cached service configuration
    /// </summary>
    internal ModernCachedServiceConfiguration? ServiceConfiguration { get; private set; }
    
    /// <summary>
    /// Collection of modern services specifications
    /// </summary>
    internal List<ModernServiceSpecification> Services { get; } = new();

    /// <summary>
    /// Collection of modern services specifications
    /// </summary>
    internal List<ModernServiceConcreteSpecification> ConcreteServices { get; } = new();
    
    /// <summary>
    /// Configures modern cached service configuration
    /// </summary>
    /// <param name="configuration">The modern cached service configuration update action</param>
    public void ConfigureService(Action<ModernCachedServiceConfiguration>? configuration = null)
    {
        ServiceConfiguration = new ModernCachedServiceConfiguration();
        configuration?.Invoke(ServiceConfiguration);
    }

    /// <summary>
    /// Adds service
    /// </summary>
    /// <param name="lifetime">Services lifetime in DI</param>
    /// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddService<TEntityDto, TEntityDbo, TId>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernServiceSpecification
        {
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId),
            Lifetime = lifetime
        };

        Services.Add(configuration);
    }

    /// <summary>
    /// Adds service
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TServiceInterface">The type of concrete service interface</typeparam>
    /// <typeparam name="TServiceImplementation">The type of concrete service implementation</typeparam>
    public void AddConcreteService<TServiceInterface, TServiceImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TServiceInterface : class
        where TServiceImplementation : class, TServiceInterface
    {
        var configuration = new ModernServiceConcreteSpecification
        {
            InterfaceType = typeof(TServiceInterface), 
            ImplementationType = typeof(TServiceImplementation), 
            Lifetime = lifetime
        };

        ConcreteServices.Add(configuration);
    }
}