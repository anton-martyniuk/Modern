using System;

namespace Modern.Services.DataStore.Cached.SourceGenerators;

/// <summary>
/// A modern cached service attribute that is used for a source generator to create
/// a service interface and implementation for a given entity.<br/>
/// Cached Service implementation is inherited from ModernCachedService.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernCachedServiceAttribute : Attribute
{
    public ModernCachedServiceAttribute(Type entityDboType)
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
