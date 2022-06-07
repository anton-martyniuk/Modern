using Modern.GraphQL.HotChocolate.DependencyInjection.Definitions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a modern GraphQL options for registering in DI
/// </summary>
public class ModernGraphQlOptions
{
    /// <summary>
    /// Collection of modern GraphQL specifications
    /// </summary>
    internal List<ModernGraphQlSpecification> GraphQlSpecifications { get; } = new();

    /// <summary>
    /// Adds GraphQL queries and mutations for specified Entity type
    /// </summary>
    /// <typeparam name="TEntityDto">The type of entity returned from the GraphQL query or mutation</typeparam>
    /// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of entity identifier</typeparam>
    public void AddQueriesAndMutationsFor<TEntityDto, TEntityDbo, TId>()
        where TEntityDto : class
        where TEntityDbo : class
        where TId : IEquatable<TId>
    {
        var configuration = new ModernGraphQlSpecification
        {
            EntityDtoType = typeof(TEntityDto),
            EntityDboType = typeof(TEntityDbo),
            EntityIdType = typeof(TId)
        };

        GraphQlSpecifications.Add(configuration);
    }
}