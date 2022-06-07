using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.GraphQL.HotChocolate.DataStore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Services.DataStore.Cached extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds HotChocolate GraphQL queries and mutations into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">GraphQL configure delegate</param>
    /// <returns>IServiceCollection</returns>
    // ReSharper disable once InconsistentNaming
    public static ModernServicesBuilder AddHotChocolateGraphQL(this ModernServicesBuilder builder, Action<ModernGraphQlOptions> configure)
    {
        var options = new ModernGraphQlOptions();
        configure(options);

        var graphQlBuilder = builder.Services
            .AddGraphQLServer()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .InitializeOnStartup();

        foreach (var c in options.GraphQlSpecifications)
        {
            var queryType = typeof(ModernGraphQlQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            graphQlBuilder.AddQueryType(queryType);

            var mutationType = typeof(ModernGraphQlMutation<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            graphQlBuilder.AddMutationType(mutationType);
        }

        return builder;
    }
}
