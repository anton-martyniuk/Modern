using Modern.Controllers.DataStore.OData.DependencyInjection.Configuration;

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
    /// <param name="apiRoute">Api route, for example: api/cities, api/cars</param>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddController<TEntityDbo, TId>(string apiRoute)
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernControllerSpecification
        {
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId),
            ApiRoute = apiRoute
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
        var configuration = new ModernControllerConcreteSpecification
        {
            ImplementationType = typeof(TControllerImplementation)
        };

        ConcreteControllers.Add(configuration);
    }
}