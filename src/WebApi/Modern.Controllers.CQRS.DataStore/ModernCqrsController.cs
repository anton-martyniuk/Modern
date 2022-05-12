using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.Exceptions;

namespace Modern.Controllers.CQRS.DataStore;

/// <summary>
/// The base entity controller for entity service
/// </summary>
[Produces("application/json")]
[Consumes("application/json")]
public class ModernCqrsController<TEntityDto, TEntityDbo, TId> : ControllerBase
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="mediator">The mediator</param>
    public ModernCqrsController(IMediator mediator)
    {
        _mediator = mediator;
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetById([Required] TId id)
    {
        try
        {
            var query = new GetByIdQuery<TEntityDto, TId>(id);
            var entity = await _mediator.Send(query).ConfigureAwait(false);
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
    /// <response code="404">Entities were not found in the data store</response>
    /// <response code="500">Error occurred while retrieving entities</response>
    [HttpGet("get")]
    //[ProducesResponseType(typeof(TEntityDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllQuery<TEntityDto, TId>();
            var entities = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
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
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> Create([FromBody, Required] TEntityDto entity)
    {
        var command = new CreateEntityCommand<TEntityDto>(entity);
        var createdEntity = await _mediator.Send(command).ConfigureAwait(false);
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
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateMany([FromBody, Required] List<TEntityDto> entities)
    {
        var command = new CreateEntitiesCommand<TEntityDto>(entities);
        var createdEntities = await _mediator.Send(command).ConfigureAwait(false);
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPut("update/{id}")]
    public virtual async Task<IActionResult> Update([Required] TId id, [FromBody, Required] TEntityDto entity)
    {
        if (!Equals(GetEntityId(entity), id))
        {
            return BadRequest("Entity 'id' doesn't match 'id' in request URL");
        }

        try
        {
            var command = new UpdateEntityCommand<TEntityDto, TId>(id, entity);
            await _mediator.Send(command).ConfigureAwait(false);
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPut("update-many")]
    public virtual async Task<IActionResult> UpdateMany([FromBody, Required] List<TEntityDto> entities)
    {
        try
        {
            var command = new UpdateEntitiesCommand<TEntityDto>(entities);
            await _mediator.Send(command).ConfigureAwait(false);
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPatch("patch/{id}")]
    public virtual async Task<IActionResult> Patch([Required] TId id, [FromBody] JsonPatchDocument<TEntityDbo> patch)
    {
        try
        {
            var command = new UpdateEntityByActionCommand<TEntityDto, TEntityDbo, TId>(id, patch.ApplyTo);
            await _mediator.Send(command).ConfigureAwait(false);
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
    /// <response code="204">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpDelete("delete/{id}")]
    public virtual async Task<IActionResult> Delete([Required] TId id)
    {
        try
        {
            var command = new DeleteEntityCommand<TId>(id);
            await _mediator.Send(command).ConfigureAwait(false);
            
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
    /// <response code="204">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpDelete("delete-many")]
    public virtual async Task<IActionResult> DeleteMany([Required] List<TId> ids)
    {
        try
        {
            var command = new DeleteEntitiesCommand<TId>(ids);
            await _mediator.Send(command).ConfigureAwait(false);
            
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
