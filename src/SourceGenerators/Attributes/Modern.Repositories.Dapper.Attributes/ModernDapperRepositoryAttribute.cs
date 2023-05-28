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
    public ModernDapperRepositoryAttribute(Type mappingType)
    {
        MappingType = mappingType;
    }

    /// <summary>
    /// Type of dapper mapping
    /// </summary>
    public Type MappingType { get; set; }
    
    public string? RepositoryName { get; set; }
}
