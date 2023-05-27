using System;

namespace Modern.Repositories.EFCore.SourceGenerators;

/// <summary>
/// An attribute that is used for a source generator to create
/// a repository interface and implementation for a given entity.<br/>
/// Repository implementation is inherited from ModernEfCoreRepositoryWithFactory.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernEfCoreRepositoryWithFactoryAttribute : Attribute
{
    public ModernEfCoreRepositoryWithFactoryAttribute(Type dbContextType)
    {
        DbContextType = dbContextType;
    }

    public Type DbContextType { get; }

    public string? RepositoryName { get; set; }
}
