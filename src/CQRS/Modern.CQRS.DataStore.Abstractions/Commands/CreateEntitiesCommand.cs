using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that creates a list of new entities in the data store
/// </summary>
/// <returns>A list of updated entities by the data store (primary key, for example)</returns>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entities in the data store</exception>
public record CreateEntitiesCommand<TEntityDto>(List<TEntityDto> Entities) : IRequest<List<TEntityDto>>
    where TEntityDto : class
{
    /// <summary>
    /// The list of entities to add to the data store
    /// </summary>
    public List<TEntityDto> Entities { get; init; } = Entities ?? throw new ArgumentNullException(nameof(Entities));
}
