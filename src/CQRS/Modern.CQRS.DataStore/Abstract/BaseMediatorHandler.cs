using MapsterMapper;
using Modern.Exceptions;
using Modern.Repositories.Abstractions.Exceptions;

namespace Modern.CQRS.DataStore.Abstract;

/// <summary>
/// The base Mediator handler definition
/// </summary>
public abstract class BaseMediatorHandler<TEntityDto, TEntityDbo>
    where TEntityDto : class
    where TEntityDbo : class
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
