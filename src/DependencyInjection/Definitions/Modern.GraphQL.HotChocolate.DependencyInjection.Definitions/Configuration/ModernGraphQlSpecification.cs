namespace Modern.GraphQL.HotChocolate.DependencyInjection.Definitions.Configuration;

/// <summary>
/// The modern GraphQL specification model
/// </summary>
public class ModernGraphQlSpecification
{
    /// <summary>
    /// The type of entity returned from the GraphQL Query or Mutation
    /// </summary>
    public Type EntityDtoType { get; set; } = default!;

    /// <summary>
    /// The type of entity contained in the data store
    /// </summary>
    public Type EntityDboType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;
}