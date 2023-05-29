using Modern.Services.DataStore.InMemory.Configuration;
using ModernServiceConcreteSpecification = Modern.Services.DataStore.InMemory.DependencyInjection.Configuration.ModernServiceConcreteSpecification;
using ModernServiceSpecification = Modern.Services.DataStore.InMemory.DependencyInjection.Configuration.ModernServiceSpecification;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern services options for registering in DI
/// </summary>
public class ModernServicesOptions
{
    /// <summary>
    /// The modern in-memory service configuration
    /// </summary>
    internal ModernInMemoryServiceConfiguration? ServiceConfiguration { get; private set; }
    
    /// <summary>
    /// Collection of modern services specifications
    /// </summary>
    internal List<ModernServiceSpecification> Services { get; } = new();

    /// <summary>
    /// Collection of modern services specifications
    /// </summary>
    internal List<ModernServiceConcreteSpecification> ConcreteServices { get; } = new();
    
    /// <summary>
    /// Configures modern in-memory service configuration
    /// </summary>
    /// <param name="configuration">The modern in-memory service configuration update action</param>
    public void ConfigureService(Action<ModernInMemoryServiceConfiguration>? configuration = null)
    {
        ServiceConfiguration = new ModernInMemoryServiceConfiguration();
        configuration?.Invoke(ServiceConfiguration);
    }

    /// <summary>
    /// Adds service
    /// </summary>
    /// <param name="lifetime">Services lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddService<TEntityDto, TEntityDbo, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
    /// <param name="lifetime">Services lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TServiceInterface">The type of concrete service interface</typeparam>
    /// <typeparam name="TServiceImplementation">The type of concrete service implementation</typeparam>
    /// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    /// 
    public void AddConcreteService<TServiceInterface, TServiceImplementation, TEntityDto, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TServiceInterface : class
        where TServiceImplementation : class, TServiceInterface
        where TEntityDto : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernServiceConcreteSpecification
        {
            InterfaceType = typeof(TServiceInterface), 
            ImplementationType = typeof(TServiceImplementation),
            EntityDtoType = typeof(TEntityDto),
            EntityIdType = typeof(TId),
            Lifetime = lifetime
        };

        ConcreteServices.Add(configuration);
    }
}