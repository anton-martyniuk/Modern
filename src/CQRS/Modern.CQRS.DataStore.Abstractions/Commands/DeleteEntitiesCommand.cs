using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Commands;

/// <summary>
/// The mediator command model that deletes the list of entities in the data store with the given list of entity ids.<br/>
/// This method does NOT query the entities from the data store before deletion.<br/>
/// If all or some of entities were not found in the data store - no exception is thrown
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided list of entities is null or has no entities in the list</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while deleting the entities in the data store</exception>
/// <returns><see langword="true"/> if all entities were deleted; otherwise, <see langword="false"/></returns>
public record DeleteEntitiesCommand<TId>(List<TId> Ids) : IRequest<bool>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The list of entity ids
    /// </summary>
    public List<TId> Ids { get; init; } = Ids ?? throw new ArgumentNullException(nameof(Ids));
}
