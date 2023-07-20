using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Modern.Controllers.DataStore.OData.DependencyInjection.Configuration;

namespace Modern.Controllers.DataStore.OData.DependencyInjection.Features;

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
        if (!controller.ControllerType.IsGenericType || genericArgs.Length != 2 || genericArgs.Length < 2)
        {
            return;
        }
        
        // Check if ApiRoute is specified
        var controllerSpecification = _controllerSpecifications.FirstOrDefault(x =>
            x.EntityDboType == genericArgs[0] && x.EntityIdType == genericArgs[1]);
        
        if (controllerSpecification is not null)
        {
            controller.Selectors.Clear();
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(controllerSpecification.ApiRoute))
            });
            
            return;
        }

        // Select the [TEntityDbo] type (1st in the list)
        // ModernODataController<**TEntityDbo**, TId>
        var genericType = genericArgs[0];

        controller.Selectors.Clear();
        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/odata/{genericType.Name}"))
        });
    }
}
