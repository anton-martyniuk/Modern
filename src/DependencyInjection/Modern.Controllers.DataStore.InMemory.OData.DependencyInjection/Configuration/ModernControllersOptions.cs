using Modern.Controllers.DependencyInjection.Definitions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern controllers options for registering in DI
/// </summary>
public class ModernControllersOptions
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
    /// <typeparam name="TEntityDto">The type of entity returned from the controller</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddController<TEntityDto, TEntityDbo, TId>()
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernControllerSpecification
        {
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId)
        };

        Controllers.Add(configuration);
    }

    /// <summary>
    /// Adds controller
    /// </summary>
    /// <typeparam name="TControllerImplementation">The type of concrete controller implementation</typeparam>
    public void AddController<TControllerImplementation>()
        where TControllerImplementation : class
    {
        // TODO: check if is assignable from types

        var configuration = new ModernControllerConcreteSpecification
        {
            ImplementationType = typeof(TControllerImplementation)
        };

        ConcreteControllers.Add(configuration);
    }
}