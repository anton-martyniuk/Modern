using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Modern.Controllers.DataStore.InMemory.DependencyInjection.Configuration;

namespace Modern.Controllers.DataStore.InMemory.DependencyInjection.Features;

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
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 5 || genericArgs.Length < 5)
        {
            return;
        }
        
        // Check if ApiRoute is specified
        var controllerSpecification = _controllerSpecifications.FirstOrDefault(x =>
            x.CreateRequestType == genericArgs[0] && x.UpdateRequestType == genericArgs[1] && x.EntityDtoType == genericArgs[2] &&
            x.EntityDboType == genericArgs[3] && x.EntityIdType == genericArgs[4]);
        
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
        // ModernInMemoryController<TCreateRequest, TUpdateRequest, **TEntityDto**, TEntityDbo, TId>
        var genericType = genericArgs[2];

        controller.Selectors.Clear();
        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/{genericType.Name}"))
        });
    }
}
