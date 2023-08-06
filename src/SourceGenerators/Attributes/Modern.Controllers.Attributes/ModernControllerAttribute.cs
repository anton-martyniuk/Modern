using System;

namespace Modern.Controllers.SourceGenerators;

/// <summary>
/// A modern controller attribute that is used for a source generator to create
/// a controller implementation for a given entity.<br/>
/// Controller implementation is inherited from ModernController.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModernControllerAttribute : Attribute
{
    public ModernControllerAttribute(Type createRequestType, Type updateRequestType, Type entityDboType,
        string apiRoute)
    {
        CreateRequestType = createRequestType;
        UpdateRequestType = updateRequestType;
        EntityDboType = entityDboType;
        ApiRoute = apiRoute;
    }
    
    /// <summary>
    /// Type a create request
    /// </summary>
    public Type CreateRequestType { get; set; }

    /// <summary>
    /// Type an update request
    /// </summary>
    public Type UpdateRequestType { get; set; }

    /// <summary>
    /// Type of dbo entity
    /// </summary>
    public Type EntityDboType { get; }

    /// <summary>
    /// Api route, for example: api/cities, api/cars
    /// </summary>
    public string? ApiRoute { get; set; }
    
    /// <summary>
    /// Custom name of the controller
    /// </summary>
    public string? ControllerName { get; set; }
}
