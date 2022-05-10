using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns an entity with the given id
/// </summary>
/// <returns>The entity</returns>
/// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
public record TryGetByIdQuery<TEntityDto, TId>(TId Id) : IRequest<TEntityDto?>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The entity id
    /// </summary>
    public TId Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
}
