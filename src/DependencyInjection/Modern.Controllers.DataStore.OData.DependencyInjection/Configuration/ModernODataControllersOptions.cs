using Modern.Controllers.DependencyInjection.Definitions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern odata controllers options for registering in DI
/// </summary>
public class ModernODataControllersOptions
{
    /// <summary>
    /// Collection of modern controllers specifications
    /// </summary>
    internal List<ModernControllerSpecification> Controllers { get; } = new();

    /// <summary>
    /// Collection of modern controllers specifications
    /// </summary>
    internal List<ModernControllerConcreteSpecification> ConcreteControllers { get; } = new();

    /// <summary>
    /// Adds controller
    /// </summary>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddController<TEntityDbo, TId>()
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernControllerSpecification
        {
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId)
        };

        Controllers.Add(configuration);
    }

    /// <summary>
    /// Adds controller
    /// </summary>
    /// <typeparam name="TServiceImplementation">The type of concrete controller implementation</typeparam>
    public void AddController<TServiceImplementation>()
        where TServiceImplementation : class
    {
        var configuration = new ModernControllerConcreteSpecification
        {
            ImplementationType = typeof(TServiceImplementation)
        };

        ConcreteControllers.Add(configuration);
    }
}