using System.Linq.Expressions;
using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that determines whether the data store contains at least one entity that matches the given predicate
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
public record GetExistsQuery<TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<bool>
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// The filtering predicate
    /// </summary>
    public Expression<Func<TEntityDbo, bool>> Predicate { get; init; } = Predicate ?? throw new ArgumentNullException(nameof(Predicate));
}
