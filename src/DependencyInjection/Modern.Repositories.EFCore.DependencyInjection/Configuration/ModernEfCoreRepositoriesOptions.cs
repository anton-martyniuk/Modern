using Microsoft.EntityFrameworkCore;
using Modern.Repositories.EFCore.DependencyInjection.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernEfCoreRepositoriesOptions
{
    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernEfCoreRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernEfCoreRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="useDbFactory">
    /// Indicates whether repository with DbContextFactory should be used.<br/>
    /// When using DbContextFactory every repository creates and closes a database connection in each method.<br/>
    /// When NOT using DbContextFactory repository shares the same database connection during its lifetime
    /// </param>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TDbContext, TEntity, TId>(bool useDbFactory = false, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TDbContext : DbContext
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernEfCoreRepositorySpecification
        {
            DbContextType = typeof(TDbContext),
            EntityType = typeof(TEntity),
            EntityIdType = typeof(TId),
            Lifetime = lifetime,
            UseDbFactory = useDbFactory
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
        var configuration = new ModernEfCoreRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}