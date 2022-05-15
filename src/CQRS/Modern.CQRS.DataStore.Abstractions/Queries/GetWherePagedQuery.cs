using System.Linq.Expressions;
using MediatR;
using Modern.Data.Paging;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns certain amount of paged entities from the data store that match the given predicate
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
/// <returns>A list of entities that match the condition</returns>
public record GetWherePagedQuery<TEntityDto, TEntityDbo, TId> : IRequest<PagedResult<TEntityDto>>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// A function to test each element for condition
    /// </summary>
    public Expression<Func<TEntityDbo, bool>> Predicate { get; init; } = default!;

    /// <summary>
    /// Page number. Entities to skip = (pageNumber - 1) * pageSize
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The total number of items to select
    /// </summary>
    public int PageSize { get; init; }
}
