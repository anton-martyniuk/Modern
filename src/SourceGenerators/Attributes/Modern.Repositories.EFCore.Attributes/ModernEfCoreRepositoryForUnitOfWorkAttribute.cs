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
    public ModernEfCoreRepositoryForUnitOfWorkAttribute(Type dbContextType)
    {
        DbContextType = dbContextType;
    }

    public Type DbContextType { get; }

    public string? RepositoryName { get; set; }
}
