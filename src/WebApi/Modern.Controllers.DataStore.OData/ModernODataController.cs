using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Modern.Repositories.Abstractions;

namespace Modern.Controllers.DataStore.OData;

/// <summary>
/// The OData controller for entity service
/// </summary>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
[Route("api/odata/[controller]")]
public class ModernODataController<TEntityDbo, TId> : ODataController
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernRepository<TEntityDbo, TId> _repository;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic query repository</param>
    //protected ModernODataController(IModernService<TEntityDto, TEntityDbo, TId> service)
    protected ModernODataController(IModernRepository<TEntityDbo, TId> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Performs an OData based query
    /// </summary>
    /// <returns>Filtered entities by OData</returns>
    /// <remarks>
    /// Request example:
    ///     GET: api/entity/get/?$select=name<br/><br/>
    /// For more information about syntax see https://docs.microsoft.com/en-us/odata/webapi/first-odata-api <br/>
    /// </remarks>
    /// <response code="200">Entities retrieved</response>
    /// <response code="500">Error occurred in the entity service</response>
    [HttpGet]
    [EnableQuery(MaxNodeCount = 1000)] // TODO: configure this ?!
    //[ProducesResponseType(typeof(IQueryable<TEntity>), (int)HttpStatusCode.OK)] TODO: // create attribute
    public virtual IActionResult Get()
    {
        return Ok(_repository.AsQueryable());
    }
}
