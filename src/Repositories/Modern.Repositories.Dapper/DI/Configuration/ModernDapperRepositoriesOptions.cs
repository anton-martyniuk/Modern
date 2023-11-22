using System.Data.Common;
using Modern.Repositories.Dapper.DI.Configuration;
using Modern.Repositories.Dapper.Mapping;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernDapperRepositoriesOptions
{
    /// <summary>
    /// A function that creates a database connection
    /// </summary>
    internal Func<DbConnection>? CreateDbConnection;

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernDapperRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernDapperRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Adds a function to create database connection.<br/>
    /// A dapper needs to know how to create a database connection. Since there are multiple database connection classes -  provide the needed one.<br />
    /// Example: <code>() => new NpgsqlConnection(connectionString)</code>
    /// </summary>
    /// <param name="createDbConnection">A function that creates a database connection</param>
    public void ProvideDatabaseConnection(Func<DbConnection> createDbConnection)
    {
        CreateDbConnection = createDbConnection;
    }

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TEntityMapping">The type of entity mapping</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TEntityMapping, TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntityMapping : DapperEntityMapping<TEntity>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernDapperRepositorySpecification
        {
            EntityMappingType = typeof(TEntityMapping),
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
    /// <typeparam name="TEntityMapping">The type of entity mapping</typeparam>
    public void AddConcreteRepository<TRepositoryInterface, TRepositoryImplementation, TEntityMapping>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TRepositoryInterface : class
        where TRepositoryImplementation : class, TRepositoryInterface
    {
        var configuration = new ModernDapperRepositoryConcreteSpecification
        {
            EntityMappingType = typeof(TEntityMapping),
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}