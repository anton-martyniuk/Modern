using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Modern.Exceptions;
using Modern.Services.Abstractions;

namespace Modern.Controllers;

/// <summary>
/// The base controller for cached service
/// </summary>
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class ModernCachedController<TEntityDto, TEntityDbo, TId> : ControllerBase
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernCachedService<TEntityDto, TEntityDbo, TId> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service">Cached service</param>
    protected ModernCachedController(IModernCachedService<TEntityDto, TEntityDbo, TId> service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity id</returns>
    // TODO: use source generators for this
    protected virtual TId GetEntityId(TEntityDto entityDto) => (TId)(entityDto.GetType().GetProperty("Id")?.GetValue(entityDto, null) ?? 0);

    /// <summary>
    /// Returns an entity with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>The entity</returns>
    /// <response code="200">Entity was found and returned</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while retrieving entity</response>
    [HttpGet("get/{id}")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)] // TODO: use Source Generator to create Attribute
    public virtual async Task<IActionResult> GetById([Required] TId id)
    {
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

    /// <summary>
    /// Returns all entities
    /// </summary>
    /// <returns>List of entities</returns>
    /// <response code="200">Entities was found and returned</response>
    /// <response code="404">Entities was not found in the data store</response>
    /// <response code="500">Error occurred while retrieving entities</response>
    [HttpGet("get")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)]
    public virtual async Task<IActionResult> GetAll()
    {
        try
        {
            var entities = await _service.GetAllAsync().ConfigureAwait(false);
            return Ok(entities);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Creates the new entity in the data store
    /// </summary>
    /// <param name="entity">The entity to add to the data store</param>
    /// <response code="201">The entity was created</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="500">Error occurred while creating entity</response>
    [HttpPost("create")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)]
    public virtual async Task<IActionResult> Create([FromBody, Required] TEntityDto entity)
    {
        var createdEntity = await _service.CreateAsync(entity).ConfigureAwait(false);
        return Created(nameof(Create), createdEntity);
    }

    /// <summary>
    /// Creates a list of new entities in the data store
    /// </summary>
    /// <param name="entities">The list of entities to add to the data store</param>
    /// <response code="201">The entities were created</response>
    /// <response code="400">One of entity models is invalid</response>
    /// <response code="500">Error occurred while creating entities</response>
    [HttpPost("create-many")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)]
    public virtual async Task<IActionResult> CreateMany([FromBody, Required] List<TEntityDto> entities)
    {
        var createdEntities = await _service.CreateAsync(entities).ConfigureAwait(false);
        return Created(nameof(Create), createdEntities);
    }

    /// <summary>
    /// Updates the entity in the data store with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">The entity model</param>
    /// <response code="204">The entity was updated</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="404">Entity with the given id not found</response>
    /// <response code="500">Error occurred while updating entity</response>
    [HttpPut("update/{id}")]
    public virtual async Task<IActionResult> Update([Required] TId id, [FromBody, Required] TEntityDto entity)
    {
        if (!Equals(GetEntityId(entity), id))
        {
            return BadRequest("Entity 'id' doesn't match 'id' in request URL");
        }

        try
        {
            await _service.UpdateAsync(id, entity).ConfigureAwait(false);
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the list of entities in the data store with the given list of <paramref name="entities"/>
    /// </summary>
    /// <param name="entities">The entity model</param>
    /// <response code="204">The entity was updated</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="404">Entity with the given id not found</response>
    /// <response code="500">Error occurred while updating entities</response>
    [HttpPut("update-many")]
    public virtual async Task<IActionResult> UpdateMany([FromBody, Required] List<TEntityDto> entities)
    {
        try
        {
            await _service.UpdateAsync(entities).ConfigureAwait(false);
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Partially updates the entity in the data store with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="patch">The json patch model that contains rules to update the entity model</param>
    /// <response code="204">The entity was updated</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="404">Entity with the given id not found</response>
    /// <response code="500">Error occurred while updating entity</response>
    [HttpPatch("patch/{id}")]
    public virtual async Task<IActionResult> Patch([Required] TId id, [FromBody] JsonPatchDocument<TEntityDto> patch)
    {
        try
        {
            await _service.UpdateAsync(id, patch.ApplyTo).ConfigureAwait(false);
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes the entity in the data store with the given <paramref name="id"/>.<br/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <response code="200">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [HttpDelete("delete/{id}")]
    public virtual async Task<IActionResult> Delete([Required] TId id)
    {
        try
        {
            await _service.DeleteAsync(id).ConfigureAwait(false);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes the list of entities in the data store with the given list of <paramref name="ids"/>.<br/>
    /// </summary>
    /// <param name="ids">The list of entity ids</param>
    /// <response code="200">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [HttpDelete("delete-many")]
    public virtual async Task<IActionResult> DeleteMany([Required] List<TId> ids)
    {
        try
        {
            await _service.DeleteAsync(ids).ConfigureAwait(false);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
