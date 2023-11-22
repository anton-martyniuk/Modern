﻿namespace Modern.Controllers.DataStore.OData.DependencyInjection.Configuration;

/// <summary>
/// The modern concrete controller specification model
/// </summary>
public class ModernControllerConcreteSpecification
{
    /// <summary>
    /// The type of concrete controller implementation
    /// </summary>
    public Type ImplementationType { get; set; } = default!;
}