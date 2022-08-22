using System.Data.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Dapper;
using Modern.Repositories.Dapper.Connection;
using Modern.Repositories.Dapper.Providers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Repositories.Dapper extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds Dapper repositories into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Repositories configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRepositoriesDapper(this ModernServicesBuilder builder, Action<ModernDapperRepositoriesOptions> configure)
    {
        var options = new ModernDapperRepositoriesOptions();
        configure(options);

        if (options.CreateDbConnection is null)
        {
            throw new InvalidOperationException("CreateDbConnection function should be provided using options.ProvideDatabaseConnection() method");
        }

        builder.Services.AddSingleton<IQueryProviderFactory, QueryProviderFactory>();
        builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(options.CreateDbConnection));

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernDapperRepository<,,>).MakeGenericType(c.EntityMappingType, c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
            builder.Services.TryAddSingleton(c.EntityMappingType);
        }

        foreach (var c in options.ConcreteRepositories)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
