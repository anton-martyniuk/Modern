using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns the total count of entities
/// </summary>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
/// <returns>Count of entities</returns>
public record GetCountAllQuery<TEntityDto, TId> : IRequest<long>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
}
