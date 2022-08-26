using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Repositories.Abstractions;
using Modern.Repositories.MongoDB;
using MongoDB.Driver;

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

        if (options.MongoClientSettings is null)
        {
            throw new InvalidOperationException("MongoDb client settings should be provided using options.ConfigureMongoDbClient() method");
        }

        builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(options.MongoClientSettings));

        foreach (var c in options.Repositories)
        {
            var interfaceType = typeof(IModernRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);
            var implementationType = typeof(ModernMongoDbRepository<,>).MakeGenericType(c.EntityType, c.EntityIdType);

            // Dynamically create an instance of repository, providing database and collection name
            builder.Services.Add(new ServiceDescriptor(interfaceType, provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();
                return ActivatorUtilities.CreateInstance(provider, implementationType, mongoClient, c.DatabaseName, c.CollectionName);
            }, c.Lifetime));
        }

        foreach (var c in options.ConcreteRepositories)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }
}
