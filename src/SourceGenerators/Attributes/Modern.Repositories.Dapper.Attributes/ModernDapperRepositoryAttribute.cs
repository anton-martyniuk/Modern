using System;

namespace Modern.Repositories.Dapper.SourceGenerators;

/// <summary>
/// A modern dapper repository attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernDapperRepository.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernDapperRepositoryAttribute : Attribute
{
    public ModernDapperRepositoryAttribute(Type mappingType, Type entityType, Type idType)
    {
        MappingType = mappingType;
        EntityType = entityType;
        IdType = idType;
    }

    /// <summary>
    /// Type of dapper mapping
    /// </summary>
    public Type MappingType { get; set; }
    
    /// <summary>
    /// Type of entity
    /// </summary>
    public Type EntityType { get; }
    
    /// <summary>
    /// Type of entity identifier
    /// </summary>
    public Type IdType { get; }
    
    /// <summary>
    /// Custom name of the repository
    /// </summary>
    public string? RepositoryName { get; set; }
}
