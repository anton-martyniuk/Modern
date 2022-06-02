using Microsoft.Extensions.DependencyInjection;
using Modern.CQRS.DependencyInjection.Definitions.Configuration;
using Modern.Repositories.Abstractions;

// ReSharper disable once CheckNamespace
namespace Modern.CQRS.DataStore.DependencyInjection.Configuration;

/// <summary>
/// Represents a modern CQRS options for registering in DI
/// </summary>
public class ModernCqrsOptions
{
    /// <summary>
    /// Collection of modern CQRS specifications
    /// </summary>
    public List<ModernCqrsSpecification> Services { get; } = new();

    /// <summary>
    /// Collection of concrete CQRS specifications
    /// </summary>
    public List<ModernCqrsConcreteSpecification> ConcreteServices { get; } = new();

    /// <summary>
    /// Adds CQRS query and handler
    /// </summary>
    /// <param name="lifetime">Services lifetime in DI</param>
    /// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    /// <typeparam name="TRepository">Type of repository used for the entity</typeparam>
    public void AddQuery<TEntityDto, TEntityDbo, TId, TRepository>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
        where TRepository : class, IModernQueryRepository<TEntityDbo, TId>, IModernCrudRepository<TEntityDbo, TId>
    {
        var configuration = new ModernCqrsSpecification
        {
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId),
            RepositoryType = typeof(TRepository),
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
    public void AddService<TServiceInterface, TServiceImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TServiceInterface : class
        where TServiceImplementation : class
    {
        // TODO: check if IModernService is assignable from types

        var configuration = new ModernCqrsConcreteSpecification
        {
            InterfaceType = typeof(TServiceInterface),
            ImplementationType = typeof(TServiceImplementation),
            Lifetime = lifetime
        };

        ConcreteServices.Add(configuration);
    }
}