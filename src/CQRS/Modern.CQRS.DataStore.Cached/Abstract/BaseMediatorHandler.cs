﻿using MapsterMapper;
using Modern.Exceptions;
using Modern.Repositories.Abstractions.Exceptions;

namespace Modern.CQRS.DataStore.Cached.Abstract;

/// <summary>
/// The base Mediator handler definition
/// </summary>
public abstract class BaseMediatorHandler<TEntityDto, TEntityDbo, TId>
    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Entity name
    /// </summary>
    protected readonly string EntityName = typeof(TEntityDto).Name;

    /// <summary>
    /// Mapper
    /// </summary>
    protected IMapper Mapper = new Mapper();

    /// <summary>
    /// Returns <typeparamref name="TEntityDto"/> mapped from <typeparamref name="TEntityDbo"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity Dbo</returns>
    protected virtual TEntityDbo MapToDbo(TEntityDto entityDto) => Mapper.Map<TEntityDbo>(entityDto);

    /// <summary>
    /// Returns <typeparamref name="TEntityDbo"/> mapped from <typeparamref name="TEntityDto"/>
    /// </summary>
    /// <param name="entityDbo">Entity Dbo</param>
    /// <returns>Entity Dto</returns>
    protected virtual TEntityDto MapToDto(TEntityDbo entityDbo) => Mapper.Map<TEntityDto>(entityDbo);

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entityDto">Entity Dto</param>
    /// <returns>Entity id</returns>
    // TODO: use source generators for this
    protected virtual TId GetEntityId(TEntityDto entityDto) => (TId)(entityDto.GetType().GetProperty("Id")?.GetValue(entityDto, null) ?? 0);

    /// <summary>
    /// Returns standardized handler exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
        => ex switch
        {
            ArgumentException _ => ex,
            EntityConcurrentUpdateException _ => ex,
            EntityAlreadyExistsException _ => ex,
            EntityNotFoundException _ => ex,
            EntityNotModifiedException _ => ex,
            _ => new RepositoryErrorException(ex.Message, ex)
        };
}