using Modern.Controllers.DataStore.InMemory.DependencyInjection.Configuration;

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
    /// <param name="apiRoute">Api route, for example: api/cities, api/cars</param>
    /// <typeparam name="TCreateRequest">The type of request that creates an entity</typeparam>
    /// <typeparam name="TUpdateRequest">The type of request that updates an entity</typeparam>
    /// <typeparam name="TEntityDto">The type of entity returned from the controller</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddController<TCreateRequest, TUpdateRequest, TEntityDto, TEntityDbo, TId>(string apiRoute)
        where TCreateRequest : class
        where TUpdateRequest : class
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernControllerSpecification
        {
            CreateRequestType = typeof(TCreateRequest),
            UpdateRequestType = typeof(TUpdateRequest),
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId),
            ApiRoute = apiRoute
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