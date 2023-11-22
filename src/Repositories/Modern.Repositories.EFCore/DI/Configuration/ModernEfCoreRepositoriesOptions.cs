using Microsoft.EntityFrameworkCore;
using Modern.Repositories.EFCore.Configuration;
using Modern.Repositories.EFCore.DI.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern repository options for registering in DI
/// </summary>
public class ModernEfCoreRepositoriesOptions
{
    /// <summary>
    /// EF Core repository configuration
    /// </summary>
    internal EfCoreRepositoryConfiguration? RepositoryConfiguration { get; private set; }
    
    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernEfCoreRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    internal List<ModernEfCoreRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();
    
    /// <summary>
    /// Configures EF Core repository configuration
    /// </summary>
    /// <param name="repositoryConfiguration">The EF Core repository configuration update action</param>
    public void ConfigureRepository(Action<EfCoreRepositoryConfiguration>? repositoryConfiguration = null)
    {
        RepositoryConfiguration = new EfCoreRepositoryConfiguration();
        repositoryConfiguration?.Invoke(RepositoryConfiguration);
    }

    /// <summary>
    /// Adds repository with injecting DbContext in the constructor.<br/>
    /// When using DbContext repository shares the same database connection during its lifetime
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TDbContext, TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
            RepositoryType = EfCoreRepositoryType.DbContext
        };

        Repositories.Add(configuration);
    }
    
    /// <summary>
    /// Adds repository with injecting DbContextFactory in the constructor.<br/>
    /// When using DbContextFactory every repository creates and closes a database connection in each method.
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepositoryWithDbFactory<TDbContext, TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
            RepositoryType = EfCoreRepositoryType.DbContextFactory
        };

        Repositories.Add(configuration);
    }
    
    /// <summary>
    /// Adds repository with injecting DbContextFactory in the constructor.<br/>
    /// When using DbContextFactory every repository creates and closes a database connection in each method.
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepositoryWithUnitOfWork<TDbContext, TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
            RepositoryType = EfCoreRepositoryType.UnitOfWork
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
        var configuration = new ModernEfCoreRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}