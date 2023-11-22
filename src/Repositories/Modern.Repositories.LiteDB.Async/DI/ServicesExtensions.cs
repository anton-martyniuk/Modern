using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.LiteDB.Async;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Repositories.LiteDB.Async extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds LiteDb Async repositories into DI.<br/>
    /// NOTE: LiteDb async repository uses litedb-async library which is not an official LiteDb project.<br/><br/>
    /// <b>DISCLAIMER:</b> Modern libraries are NOT responsible for any problems with litedb-async library, so use this package at your own risk.<br/><br/>
    /// For more information about litedb-async see here: https://github.com/mlockett42/litedb-async
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Repositories configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddRepositoriesLiteDbAsync(this ModernServicesBuilder builder, Action<ModernLiteDbAsyncRepositoriesOptions> configure)
    {
        var options = new ModernLiteDbAsyncRepositoriesOptions();
        configure(options);

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernLiteDbAsyncRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);

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
