using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that Updates the list of entities in the data store with the given list of entities.<br/>
/// If all or some of entities were not found in the data store - no exception is thrown
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
/// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entities in the data store</exception>
/// <returns>List of updated entities</returns>
public record UpdateEntitiesCommand<TEntityDto>(List<TEntityDto> Entities) : IRequest<List<TEntityDto>>
    where TEntityDto : class
{
    /// <summary>
    /// The list of entities which should be updated in the data store
    /// </summary>
    public List<TEntityDto> Entities { get; init; } = Entities ?? throw new ArgumentNullException(nameof(Entities));
}
