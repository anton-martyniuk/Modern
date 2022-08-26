using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that deletes the entity in the data store with the given entity id.<br/>
/// This method does NOT query the entity from the data store before deletion
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entity in the data store</exception>
/// <returns><see langword="true"/> if entity was deleted; otherwise, <see langword="false"/></returns>
public record DeleteEntityCommand<TId>(TId Id) : IRequest<bool>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The entity id
    /// </summary>
    public TId Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
}
