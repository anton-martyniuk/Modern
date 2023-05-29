using System;

namespace Modern.Repositories.EFCore.SourceGenerators;

/// <summary>
/// A modern EF Core repository for UnitOfWork attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernEfCoreRepositoryForUnitOfWork.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernEfCoreRepositoryForUnitOfWorkAttribute : Attribute
{
    public ModernEfCoreRepositoryForUnitOfWorkAttribute(Type dbContextType, Type entityType, Type idType)
    {
        DbContextType = dbContextType;
        EntityType = entityType;
        IdType = idType;
    }

    /// <summary>
    /// Type of EF Core db context
    /// </summary>
    public Type DbContextType { get; }
    
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
