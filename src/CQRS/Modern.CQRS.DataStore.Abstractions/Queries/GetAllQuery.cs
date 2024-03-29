﻿using MediatR;
using Modern.Exceptions;

namespace Modern.CQRS.DataStore.Abstractions.Queries;

/// <summary>
/// The mediator query model that returns all entities.<br/>
/// IMPORTANT: there can be performance issues when retrieving large amount of entities
/// </summary>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
/// <returns>A list of all entities</returns>
public record GetAllQuery<TEntityDto, TId> : IRequest<List<TEntityDto>>
    where TEntityDto : class
    where TId : IEquatable<TId>
{
}
