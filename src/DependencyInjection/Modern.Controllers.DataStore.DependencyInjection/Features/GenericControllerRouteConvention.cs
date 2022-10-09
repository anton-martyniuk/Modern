using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Modern.Controllers.DataStore.DependencyInjection.Features;

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
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 5)
        {
            return;
        }

        // Select the TEntityDto type (3rd in the list)
        // ModernController<TCreateRequest, TUpdateRequest, **TEntityDto**, TEntityDbo, TId>
        var genericType = genericArgs[2];

        controller.Selectors.Clear();

        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/{genericType.Name}"))
        });
    }
}
