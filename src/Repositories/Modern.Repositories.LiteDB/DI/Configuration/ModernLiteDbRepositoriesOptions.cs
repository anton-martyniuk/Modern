using Modern.Repositories.LiteDB.DI.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernLiteDbRepositoriesOptions
{
    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernLiteDbRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernLiteDbRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="connectionString">Connection string to the LiteDB database</param>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TEntity, TId>(string connectionString, string collectionName, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernLiteDbRepositorySpecification
        {
            ConnectionString = connectionString,
            CollectionName = collectionName,
            EntityType = typeof(TEntity),
            EntityIdType = typeof(TId),
            Lifetime = lifetime
        };

        Repositories.Add(configuration);
    }

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TRepositoryInterface">The type of concrete repository interface</typeparam>
    /// <typeparam name="TRepositoryImplementation">The type of concrete repository implementation</typeparam>
    public void AddConcreteRepository<TRepositoryInterface, TRepositoryImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TRepositoryInterface : class
        where TRepositoryImplementation : class, TRepositoryInterface
    {
        var configuration = new ModernLiteDbRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}