using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Modern.Controllers.DataStore.OData.DependencyInjection.Features;

/// <summary>
/// The <see cref="IControllerModelConvention"/> implementation that specifies the [Routes] of auto-registered Modern controllers.<br/>
/// The default controller route is: "api/EntityDtoName"
/// </summary>
internal class GenericControllerRouteConvention : IControllerModelConvention
{
    /// <summary>
    /// <inheritdoc cref="IControllerModelConvention.Apply"/>
    /// </summary>
    public void Apply(ControllerModel controller)
    {
        var genericArgs = controller.ControllerType.GenericTypeArguments;
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 2)
        {
            return;
        }

        // Select the [TEntityDto] type (1st in the list)
        // ModernODataController<**TEntityDbo**, TId>
        var genericType = genericArgs[0];

        controller.Selectors.Clear();

        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/{genericType.Name}"))
        });
    }
}
