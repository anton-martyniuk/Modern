using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.MongoDB;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Repositories.MongoDB extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds MongoDb repositories into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Repositories configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRepositoriesMongoDb(this ModernServicesBuilder builder, Action<ModernMongoDbRepositoriesOptions> configure)
    {
        var options = new ModernMongoDbRepositoriesOptions();
        configure(options);

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernMongoDbRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        foreach (var c in options.ConcreteRepositories)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
