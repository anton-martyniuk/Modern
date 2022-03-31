using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Services.Abstractions;

namespace Modern.Controllers;

/// <summary>
/// The base controller for Modern applications
/// </summary>
[ApiController]
[Route("api/configs")]
[Produces("application/json")]
[Consumes("application/json")]
public class ModernController<TEntityDto, TEntityDbo, TId, TRepository> : ControllerBase
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>, IModernCrudRepository<TEntityDbo, TId>
{
    private readonly IModernEntityService<TEntityDto, TEntityDbo, TId, TRepository> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service"></param>
    public ModernController(IModernEntityService<TEntityDto, TEntityDbo, TId, TRepository> service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns an entity with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>The entity</returns>
    /// <remarks>
    /// Request example:
    ///     GET: api/configs/get/{key}
    /// </remarks>
    /// <response code="200">Configuration parameter retrieved</response>
    /// <response code="500">Error occurred in the configuration provider</response>
    [HttpGet("get/{id}")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetById([Required] TId id)
    {
        // getbyid from service
        try
        {
            var entity = await _service.GetByIdAsync(id).ConfigureAwait(false);
            return Ok(entity);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // TODO: Create, CreateMany, Update, Update (patch action) UpdateMany, Delete, DeleteMany
}
