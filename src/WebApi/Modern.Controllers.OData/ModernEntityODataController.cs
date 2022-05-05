﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modern.Services.Abstractions;

namespace Modern.Controllers.OData;

/// <summary>
/// The OData controller for entity service
/// </summary>
[Produces("application/json")]
[Consumes("application/json")]
public abstract class ModernEntityODataController<TEntityDto, TEntityDbo, TId> : ControllerBase
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernEntityService<TEntityDto, TEntityDbo, TId> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service">Entity service</param>
    protected ModernEntityODataController(IModernEntityService<TEntityDto, TEntityDbo, TId> service)
    {
        _service = service;
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
    //[ProducesResponseType(typeof(IQueryable<TEntity>), (int)HttpStatusCode.OK)] TODO: // create attribute
    public virtual IActionResult Get()
    {
        return Ok(_service.AsQueryable());
    }
}
