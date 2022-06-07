using System.ComponentModel.DataAnnotations;
using HotChocolate.Types;
using Microsoft.AspNetCore.JsonPatch;
using Modern.Exceptions;
using Modern.Services.DataStore.Abstractions;

namespace Modern.GraphQL.HotChocolate.DataStore;

/// <summary>
/// The modern GraphQL mutation model
/// </summary>
/// <typeparam name="TEntityDto">The type of entity returned from the service</typeparam>
/// <typeparam name="TEntityDbo">The type of entity contained in the data store</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernGraphQlMutation<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    private readonly IModernService<TEntityDto, TEntityDbo, TId> _service;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="service">Entity service</param>
    public ModernGraphQlMutation(IModernService<TEntityDto, TEntityDbo, TId> service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates the new entity in the data store
    /// </summary>
    /// <param name="entity">The entity to add to the data store</param>
    public virtual async Task<TEntityDto> Create(TEntityDto entity)
    {
        var createdEntity = await _service.CreateAsync(entity).ConfigureAwait(false);
        return createdEntity;
    }

    /// <summary>
    /// Creates a list of new entities in the data store
    /// </summary>
    /// <param name="entities">The list of entities to add to the data store</param>
    public virtual async Task<List<TEntityDto>> CreateMany(List<TEntityDto> entities)
    {
        var createdEntities = await _service.CreateAsync(entities).ConfigureAwait(false);
        return createdEntities;
    }

    /// <summary>
    /// Updates the entity in the data store with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">The entity model</param>
    [Error(typeof(ValidationException))]
    [Error(typeof(EntityNotFoundException))]
    public virtual async Task<TEntityDto> Update(TId id, TEntityDto entity)
    {
        var updatedEntity = await _service.UpdateAsync(id, entity).ConfigureAwait(false);
        return updatedEntity;
    }

    /// <summary>
    /// Updates the list of entities in the data store with the given list of <paramref name="entities"/>
    /// </summary>
    /// <param name="entities">The entity model</param>
    public virtual async Task<List<TEntityDto>> UpdateMany(List<TEntityDto> entities)
    {
        var updatedEntities = await _service.UpdateAsync(entities).ConfigureAwait(false);
        return updatedEntities;
    }

    /// <summary>
    /// Deletes the entity in the data store with the given <paramref name="id"/>.<br/>
    /// </summary>
    /// <param name="id">The entity id</param>
    public virtual async Task<bool> Delete(TId id)
    {
        var result = await _service.DeleteAsync(id).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Deletes the list of entities in the data store with the given list of <paramref name="ids"/>.<br/>
    /// </summary>
    /// <param name="ids">The list of entity ids</param>
    public virtual async Task<bool> DeleteMany(List<TId> ids)
    {
        var result = await _service.DeleteAsync(ids).ConfigureAwait(false);
        return result;
    }
}
