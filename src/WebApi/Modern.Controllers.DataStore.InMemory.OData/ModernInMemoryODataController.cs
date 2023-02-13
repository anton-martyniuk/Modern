using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;

namespace Modern.Controllers.DataStore.InMemory.OData;

/// <summary>
/// The OData controller for cached service
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
[Route("api/odata/[controller]")]
public class ModernInMemoryODataController<TEntityDto, TId> : ODataController
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    private readonly IModernServiceCache<TEntityDto, TId> _cache;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="cache">Cache</param>
    public ModernInMemoryODataController(IModernServiceCache<TEntityDto, TId> cache)
    {
        _cache = cache;
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
    [EnableQuery(MaxNodeCount = 1000)]
    //[ProducesResponseType(typeof(IEnumerable<TEntity>), (int)HttpStatusCode.OK)] TODO: // create attribute
    public virtual IActionResult Get()
    {
        return Ok(_cache.AsEnumerable());
    }
}
