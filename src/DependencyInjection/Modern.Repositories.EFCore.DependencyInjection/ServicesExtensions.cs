using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.EFCore;
using Modern.Repositories.EFCore.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Repositories.EFCore extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds EFCore repositories into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Repositories configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRepositoriesEfCore(this ModernServicesBuilder builder, Action<ModernEfCoreRepositoriesOptions> configure)
    {
        var options = new ModernEfCoreRepositoriesOptions();
        configure(options);

        builder.Services.Configure<EfCoreRepositoryConfiguration>(x =>
        {
            x.CreateConfiguration = options.RepositoryConfiguration.CreateConfiguration;
            x.UpdateConfiguration = options.RepositoryConfiguration.UpdateConfiguration;
            x.DeleteConfiguration = options.RepositoryConfiguration.DeleteConfiguration;
        });

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = c.UseDbFactory 
                ? typeof(ModernEfCoreRepositoryWithFactory<,,>).MakeGenericType(c.DbContextType, c.EntityType, c.EntityIdType)
                : typeof(ModernEfCoreRepository<,,>).MakeGenericType(c.DbContextType, c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        foreach (var c in options.ConcreteRepositories)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
