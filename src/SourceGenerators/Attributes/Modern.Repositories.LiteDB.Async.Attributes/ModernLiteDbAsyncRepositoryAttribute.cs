using System;

namespace Modern.Repositories.LiteDB.Async.SourceGenerators;

/// <summary>
/// A modern LiteDB Async repository attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernLiteDbAsyncRepository.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernLiteDbAsyncRepositoryAttribute : Attribute
{
    public ModernLiteDbAsyncRepositoryAttribute(string connectionString, string collectionName)
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
