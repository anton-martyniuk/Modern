using Microsoft.Extensions.DependencyInjection;

namespace Modern.Repositories.MongoDB.DI.Configuration;

/// <summary>
/// The modern repository specification model
/// </summary>
public class ModernMongoDbRepositorySpecification
{
    /// <summary>
    /// Name of the database
    /// </summary>
    public string DatabaseName { get; set; } = default!;

    /// <summary>
    /// Name of the collection
    /// </summary>
    public string CollectionName { get; set; } = default!;

    /// <summary>
    /// The type of entity contained in the data store
    /// </summary>
    public Type EntityType { get; set; } = default!;

    /// <summary>
    /// The type of entity identifier
    /// </summary>
    public Type EntityIdType { get; set; } = default!;

    /// <summary>
    /// Repository lifetime in DI
    /// </summary>
    public ServiceLifetime Lifetime { get; set; }
}