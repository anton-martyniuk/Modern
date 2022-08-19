using Modern.Repositories.MongoDB.DependencyInjection.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernMongoDbRepositoriesOptions
{
    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    public List<ModernMongoDbRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    public List<ModernMongoDbRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernMongoDbRepositorySpecification
        {
            EntityType = typeof(TEntity),
            EntityIdType = typeof(TId),
            Lifetime = lifetime
        };

        Repositories.Add(configuration);
    }

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TRepositoryInterface">The type of concrete repository interface</typeparam>
    /// <typeparam name="TRepositoryImplementation">The type of concrete repository implementation</typeparam>
    public void AddConcreteRepository<TRepositoryInterface, TRepositoryImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TRepositoryInterface : class
        where TRepositoryImplementation : class
    {
        // TODO: check if IModernService is assignable from types

        var configuration = new ModernMongoDbRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}