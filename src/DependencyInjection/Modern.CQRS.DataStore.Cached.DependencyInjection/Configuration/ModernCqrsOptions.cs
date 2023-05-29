using Modern.CQRS.DataStore.Cached.DependencyInjection.Configuration;
using Modern.Repositories.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern CQRS options for registering in DI
/// </summary>
public class ModernCqrsOptions
{
    /// <summary>
    /// Collection of modern CQRS specifications
    /// </summary>
    internal List<ModernCqrsSpecification> CqrsRequests { get; } = new();

    /// <summary>
    /// Adds CQRS queries, commands and their handlers
    /// </summary>
    /// <param name="lifetime">CQRS lifetime in DI (Scoped by default)</param>
    /// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    /// <typeparam name="TRepository">Type of repository used for the entity</typeparam>
    public void AddQueriesCommandsAndHandlersFor<TEntityDto, TEntityDbo, TId, TRepository>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
        where TRepository : class, IModernQueryRepository<TEntityDbo, TId>, IModernCrudRepository<TEntityDbo, TId>
    {
        var configuration = new ModernCqrsSpecification
        {
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId),
            RepositoryType = typeof(TRepository),
            Lifetime = lifetime
        };

        CqrsRequests.Add(configuration);
    }
}