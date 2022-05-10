using System.Linq.Expressions;
using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns the total count of entities that match the given predicate
/// </summary>
/// <returns>Count of entities</returns>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
public record GetCountQuery<TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<int>
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// A function to test each element for condition
    /// </summary>
    public Expression<Func<TEntityDbo, bool>> Predicate { get; init; } = Predicate ?? throw new ArgumentNullException(nameof(Predicate));
}
