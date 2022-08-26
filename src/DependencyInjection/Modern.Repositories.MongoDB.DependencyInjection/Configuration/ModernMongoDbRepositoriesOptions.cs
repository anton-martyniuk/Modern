using Modern.Repositories.MongoDB.DependencyInjection.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernMongoDbRepositoriesOptions
{
    /// <summary>
    /// The settings for a MongoDb client
    /// </summary>
    internal MongoClientSettings? MongoClientSettings { get; set; }

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernMongoDbRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernMongoDbRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Configures MongoDb Client settings
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <param name="updateSettings">The MongoDb client settings update action</param>
    public void ConfigureMongoDbClient(string connectionString, Action<MongoClientSettings>? updateSettings = null)
    {
        MongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
        MongoClientSettings.LinqProvider = LinqProvider.V3;

        updateSettings?.Invoke(MongoClientSettings);
    }

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <param name="databaseName">Name of the database</param>
    /// <param name="collectionName">Name of the collection</param>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TEntity, TId>(string databaseName, string collectionName, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernMongoDbRepositorySpecification
        {
            DatabaseName = databaseName,
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
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TRepositoryInterface">The type of concrete repository interface</typeparam>
    /// <typeparam name="TRepositoryImplementation">The type of concrete repository implementation</typeparam>
    public void AddConcreteRepository<TRepositoryInterface, TRepositoryImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TRepositoryInterface : class
        where TRepositoryImplementation : class, TRepositoryInterface
    {
        var configuration = new ModernMongoDbRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface), 
            ImplementationType = typeof(TRepositoryImplementation), 
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}