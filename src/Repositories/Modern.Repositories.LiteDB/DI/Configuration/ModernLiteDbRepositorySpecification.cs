﻿using Microsoft.Extensions.DependencyInjection;

namespace Modern.Repositories.LiteDB.DI.Configuration;

/// <summary>
/// The modern repository specification model
/// </summary>
public class ModernLiteDbRepositorySpecification
{
    /// <summary>
    /// Connection string to the LiteDB database
    /// </summary>
    public string ConnectionString { get; set; } = default!;

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