using System;

namespace Modern.Repositories.MongoDB.SourceGenerators;

/// <summary>
/// A modern MongoDB repository attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernMongoDbRepository.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernMongoDbRepositoryAttribute : Attribute
{
    public ModernMongoDbRepositoryAttribute(string databaseName, string collectionName)
    {
        DatabaseName = databaseName;
        CollectionName = collectionName;
    }

    public string? RepositoryName { get; set; }

    /// <summary>
    /// Name of the MongoDB database
    /// </summary>
    public string DatabaseName { get; set; }
    
    /// <summary>
    /// Name of the MongoDB collection
    /// </summary>
    public string CollectionName { get; set; }
}
