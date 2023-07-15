using System;

namespace Modern.Services.DataStore.SourceGenerators;

/// <summary>
/// A modern service attribute that is used for a source generator to create
/// a service interface and implementation for a given entity.<br/>
/// Service implementation is inherited from ModernService.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernServiceAttribute : Attribute
{
    public ModernServiceAttribute(Type entityDboType)
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
