using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that updates the entity in the data store with the given entity id
/// </summary>
/// <returns>Updated entity</returns>
/// <exception cref="ArgumentNullException">Thrown if provided id or entity is null</exception>
/// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
/// <exception cref="EntityConcurrentUpdateException">If an entity concurrent update occurred</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while updating the entity in the data store</exception>
public record UpdateEntityCommand<TEntityDto, TId>(TId Id, TEntityDto Entity) : IRequest<TEntityDto>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The entity id
    /// </summary>
    public TId Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));

    /// <summary>
    /// The entity to add to the data store
    /// </summary>
    public TEntityDto Entity { get; init; } = Entity ?? throw new ArgumentNullException(nameof(Entity));
}
