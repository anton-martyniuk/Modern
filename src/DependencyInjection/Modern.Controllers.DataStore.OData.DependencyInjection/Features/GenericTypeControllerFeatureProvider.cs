using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Modern.Controllers.DataStore.OData.DependencyInjection.Features;

/// <summary>
/// The <see cref="IApplicationFeatureProvider"/> implementation that auto-registers Modern controllers.<br/>
/// The default controller route is: "api/EntityDtoName"
/// </summary>
internal class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly ModernODataControllersOptions _options;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="options">Modern controller options</param>
    internal GenericTypeControllerFeatureProvider(ModernODataControllersOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// <inheritdoc cref="IApplicationFeatureProvider{T}.PopulateFeature"/>
    /// </summary>
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        // Non concrete controllers
        var controllers = _options.Controllers.Where(x => _options.ConcreteControllers.All(t => !x.GetType().GetTypeInfo().IsInstanceOfType(t))).ToList();
        foreach (var c in controllers)
        {
            var implementationType = typeof(ModernODataController<,>).MakeGenericType(c.EntityDboType, c.EntityIdType);
            feature.Controllers.Add(implementationType.GetTypeInfo());
        }

        foreach (var c in _options.ConcreteControllers)
        {
            feature.Controllers.Add(c.GetType().GetTypeInfo());
        }
    }
}
