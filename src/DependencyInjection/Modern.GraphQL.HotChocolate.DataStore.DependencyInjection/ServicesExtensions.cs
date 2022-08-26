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
            //.ModifyOptions(x => x.StrictValidation = false)
            .InitializeOnStartup();

        var firstSpecification = options.GraphQlSpecifications.FirstOrDefault();
        if (firstSpecification is not null)
        {
            var s = firstSpecification;
            var queryType = typeof(ModernGraphQlQuery<,,>).MakeGenericType(s.EntityDtoType, s.EntityDboType, s.EntityIdType);
            graphQlBuilder.AddQueryType(queryType);

            var mutationType = typeof(ModernGraphQlMutation<,,>).MakeGenericType(s.EntityDtoType, s.EntityDboType, s.EntityIdType);
            graphQlBuilder.AddMutationType(mutationType);
        }

        //graphQlBuilder.AddQueryType(x => x.Name("Query"));
        //graphQlBuilder.AddMutationType(x => x.Name("Mutation"));

        foreach (var c in options.GraphQlSpecifications.Skip(1))
        {
            var queryType = typeof(ModernGraphQlQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            graphQlBuilder.AddType(queryType);

            var mutationType = typeof(ModernGraphQlMutation<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            graphQlBuilder.AddType(mutationType);
        }

        return builder;
    }
}
