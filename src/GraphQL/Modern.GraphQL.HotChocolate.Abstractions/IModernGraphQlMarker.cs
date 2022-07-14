using HotChocolate.Types;

namespace Modern.GraphQL.HotChocolate.Abstractions;

/// <summary>
/// The GraphQL marker interfaced intended for extending multiple types into a single GraphQL Query<br/>
/// Note: Only one query type can be registered using AddQueryType().
/// If we want to split up our query type into multiple classes, we can do so using type extensions.<br/>
/// Learn more about object types - https://chillicream.com/docs/hotchocolate/defining-a-schema/extending-types
/// </summary>
[InterfaceType]
public interface IModernGraphQlMarker
{
}
