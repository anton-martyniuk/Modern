using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns an entity with the given id
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided id is null</exception>
/// <exception cref="EntityNotFoundException">Thrown if an entity does is not found</exception>
/// <exception cref="InternalErrorException">If a service internal error occurred</exception>
/// <returns>The entity</returns>
public record GetByIdQuery<TEntityDto, TId>(TId Id) : IRequest<TEntityDto>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The entity id
    /// </summary>
    public TId Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
}
