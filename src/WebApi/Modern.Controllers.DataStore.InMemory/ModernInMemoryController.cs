﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Modern.Exceptions;
using Modern.Services.DataStore.InMemory.Abstractions;

namespace Modern.Controllers.DataStore.InMemory;

/// <summary>
/// The base controller for cached service
/// </summary>
/// <typeparam name="TCreateRequest">The type of request that creates an entity</typeparam>
/// <typeparam name="TUpdateRequest">The type of request that updates an entity</typeparam>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
[Route("api/[controller]")]
public class ModernInMemoryController<TCreateRequest, TUpdateRequest, TEntityDto, TEntityDbo, TId> : ControllerBase
    where TCreateRequest : class
    where TUpdateRequest : class
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernInMemoryService<TEntityDto, TEntityDbo, TId> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service">Cached service</param>
    public ModernInMemoryController(IModernInMemoryService<TEntityDto, TEntityDbo, TId> service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns an entity with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <returns>The entity</returns>
    /// <response code="200">Entity was found and returned</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while retrieving entity</response>
    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
    /// <response code="404">Entities were not found in the data store</response>
    /// <response code="500">Error occurred while retrieving entities</response>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return Ok(entities);
    }

    /// <summary>
    /// Creates the new entity in the data store
    /// </summary>
    /// <param name="request">The request that creates an entity</param>
    /// <response code="201">The entity was created</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="500">Error occurred while creating entity</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> Create([FromBody, Required] TCreateRequest request)
    {
        var entity = request.Adapt<TEntityDto>();
        var createdEntity = await _service.CreateAsync(entity).ConfigureAwait(false);
        return Created(nameof(Create), createdEntity);
    }

    /// <summary>
    /// Creates a list of new entities in the data store
    /// </summary>
    /// <param name="requests">The list of requests that create entities</param>
    /// <response code="201">The entities were created</response>
    /// <response code="400">One of entity models is invalid</response>
    /// <response code="500">Error occurred while creating entities</response>
    [HttpPost("create-many")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateMany([FromBody, Required] List<TCreateRequest> requests)
    {
        var entities = requests.Adapt<List<TEntityDto>>();
        var createdEntities = await _service.CreateAsync(entities).ConfigureAwait(false);
        return Created(nameof(Create), createdEntities);
    }

    /// <summary>
    /// Updates the entity in the data store with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="request">The request that updates an entity</param>
    /// <response code="204">The entity was updated</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="404">Entity with the given id not found</response>
    /// <response code="500">Error occurred while updating entity</response>
    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> Update([Required] TId id, [FromBody, Required] TUpdateRequest request)
    {
        try
        {
            var entity = request.Adapt<TEntityDto>();
            await _service.UpdateAsync(id, entity).ConfigureAwait(false);
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the list of entities in the data store with the given list of update <paramref name="requests"/>
    /// </summary>
    /// <param name="requests">The list of requests that updates entities</param>
    /// <response code="204">The entity was updated</response>
    /// <response code="400">The entity model is invalid</response>
    /// <response code="404">Entity with the given id not found</response>
    /// <response code="500">Error occurred while updating entities</response>
    [HttpPut("update-many")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> UpdateMany([FromBody, Required] List<TUpdateRequest> requests)
    {
        try
        {
            var entities = requests.Adapt<List<TEntityDto>>();
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
    [HttpPatch("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
    /// <response code="204">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> Delete([Required] TId id)
    {
        var result = await _service.DeleteAsync(id).ConfigureAwait(false);
        if (!result)
        {
            var entityName = typeof(TEntityDto).Name;
            return NotFound($"{entityName} entity with id '{id}' not found");
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes the list of entities in the data store with the given list of <paramref name="ids"/>.<br/>
    /// </summary>
    /// <param name="ids">The list of entity ids</param>
    /// <response code="204">Entity was found and deleted from the data store</response>
    /// <response code="404">Entity was not found in the data store</response>
    /// <response code="500">Error occurred while deleting entity</response>
    [HttpDelete("delete-many")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteMany([Required] List<TId> ids)
    {
        var result = await _service.DeleteAsync(ids).ConfigureAwait(false);
        if (!result)
        {
            var entityName = typeof(TEntityDto).Name;
            return NotFound($"Not all {entityName} entities were found for deletion");
        }

        return NoContent();
    }
}
