using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that creates the new entity
/// </summary>
/// <returns>Updated entity by the data store (primary key, for example)</returns>
/// <exception cref="ArgumentNullException">Thrown if provided entity is null</exception>
/// <exception cref="EntityAlreadyExistsException">Thrown if an entity already exists in the data store</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while saving the entity in the data store</exception>
public record CreateEntityCommand<TEntityDto>(TEntityDto Entity) : IRequest<TEntityDto>
    where TEntityDto : class
{
    /// <summary>
    /// The entity to add to the data store
    /// </summary>
    public TEntityDto Entity { get; init; } = Entity ?? throw new ArgumentNullException(nameof(Entity));
}
