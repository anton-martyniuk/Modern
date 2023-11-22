using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Modern.Controllers.CQRS.DataStore.DI.Configuration;

namespace Modern.Controllers.CQRS.DataStore.DI.Features;

/// <summary>
/// The <see cref="IControllerModelConvention"/> implementation that specifies the [Routes] of auto-registered Modern controllers.<br/>
/// The default controller route is: "api/EntityDtoName"
/// </summary>
internal class GenericControllerRouteConvention : IControllerModelConvention
{
    private readonly IReadOnlyList<ModernControllerSpecification> _controllerSpecifications;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="controllerSpecifications">Collection of controller specifications</param>
    public GenericControllerRouteConvention(IReadOnlyList<ModernControllerSpecification> controllerSpecifications)
    {
        _controllerSpecifications = controllerSpecifications;
    }

    /// <summary>
    /// <inheritdoc cref="IControllerModelConvention.Apply"/>
    /// </summary>
    public void Apply(ControllerModel controller)
    {
        var genericArgs = controller.ControllerType.GenericTypeArguments;
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 4 || genericArgs.Length < 4)
        {
            return;
        }
        
        // Check if ApiRoute is specified
        var controllerSpecification = _controllerSpecifications.FirstOrDefault(x =>
            x.CreateRequestType == genericArgs[0] && x.UpdateRequestType == genericArgs[1] && x.EntityDtoType == genericArgs[2] &&
            x.EntityIdType == genericArgs[3]);
        
        if (controllerSpecification is not null)
        {
            controller.Selectors.Clear();
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(controllerSpecification.ApiRoute))
            });

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
