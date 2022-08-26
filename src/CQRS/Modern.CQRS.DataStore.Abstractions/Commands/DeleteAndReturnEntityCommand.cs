using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that deletes and returns an entity in the data store with the given entity id.<br/>
/// This method queries the entity from the data store before deletion
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
/// <exception cref="EntityNotFoundException">Thrown if an entity does not exist in the data store</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entity in the data store</exception>
/// <returns>Deleted entity</returns>
public record DeleteAndReturnEntityCommand<TEntityDto, TId>(TId Id) : IRequest<TEntityDto>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The entity id
    /// </summary>
    public TId Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
}
