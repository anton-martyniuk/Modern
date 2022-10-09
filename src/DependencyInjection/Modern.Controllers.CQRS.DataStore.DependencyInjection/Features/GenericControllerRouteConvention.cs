using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Modern.Controllers.CQRS.DataStore.DependencyInjection.Features;

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
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 4)
        {
            return;
        }

        // Select the TEntityDto type (3rd in the list)
        // ModernCqrsController<TCreateRequest, TUpdateRequest, **TEntityDto**, TId>
        var genericType = genericArgs[2];

        controller.Selectors.Clear();

        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/{genericType.Name}"))
        });
    }
}
