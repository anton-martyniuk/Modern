using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.LiteDB;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Repositories.MongoDB extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds LiteDb repositories into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Repositories configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRepositoriesLiteDb(this ModernServicesBuilder builder, Action<ModernLiteDbRepositoriesOptions> configure)
    {
        var options = new ModernLiteDbRepositoriesOptions();
        configure(options);

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernLiteDbRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            // Dynamically create an instance of repository, providing connection string and collection name
            builder.Services.Add(new ServiceDescriptor(interfaceType,
                provider => ActivatorUtilities.CreateInstance(provider, implementationType, c.ConnectionString, c.CollectionName), c.Lifetime)
            );
        }

        foreach (var c in options.ConcreteRepositories)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
