using System;

namespace Modern.Services.DataStore.InMemory.SourceGenerators;

/// <summary>
/// A modern in memory service attribute that is used for a source generator to create
/// a service interface and implementation for a given entity.<br/>
/// InMemory Service implementation is inherited from ModernInMemoryService.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernInMemoryServiceAttribute : Attribute
{
    public ModernInMemoryServiceAttribute(Type entityDboType)
    {
        EntityDboType = entityDboType;
    }

    /// <summary>
    /// Type of dbo entity
    /// </summary>
    public Type EntityDboType { get; }

    /// <summary>
    /// Custom name of the service
    /// </summary>
    public string? ServiceName { get; set; }
}
