using System;

namespace Modern.Repositories.LiteDB.SourceGenerators;

/// <summary>
/// A modern LiteDB repository attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernLiteDbRepository.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernLiteDbRepositoryAttribute : Attribute
{
    public ModernLiteDbRepositoryAttribute(string connectionString, string collectionName)
    {
        ConnectionString = connectionString;
        CollectionName = collectionName;
    }

    public string? RepositoryName { get; set; }

    /// <summary>
    /// Connection string to the LiteDB database
    /// </summary>
    public string ConnectionString { get; set; }
    
    /// <summary>
    /// Name of the LiteDB collection
    /// </summary>
    public string CollectionName { get; set; }
}
