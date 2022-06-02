﻿using Microsoft.EntityFrameworkCore;
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
    public List<ModernEfCoreRepositorySpecification> Repositories { get; } = new();

    /// <summary>
    /// Collection of modern repository specifications
    /// </summary>
    public List<ModernEfCoreRepositoryConcreteSpecification> ConcreteRepositories { get; } = new();

    /// <summary>
    /// Adds repository
    /// </summary>
    /// <param name="lifetime">Repository lifetime in DI</param>
    /// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddRepository<TDbContext, TEntity, TId>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TDbContext : DbContext
        where TEntity : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernEfCoreRepositorySpecification
        {
            DbContextType = typeof(TDbContext),
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
    public void AddRepository<TRepositoryInterface, TRepositoryImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TRepositoryInterface : class
        where TRepositoryImplementation : class
    {
        // TODO: check if IModernService is assignable from types

        var configuration = new ModernEfCoreRepositoryConcreteSpecification
        {
            InterfaceType = typeof(TRepositoryInterface),
            ImplementationType = typeof(TRepositoryImplementation),
            Lifetime = lifetime
        };

        ConcreteRepositories.Add(configuration);
    }
}