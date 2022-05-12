using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.QueryHandlers;

/// <summary>
/// The mediator query handler that returns certain amount of paged entities from the data store that match the given predicate
/// </summary>
/// <returns>A list of entities that match the condition</returns>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
public class GetWherePagedQueryHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<GetWherePagedQuery<TEntityDto, TEntityDbo, TId>, PagedResult<TEntityDto>>

    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(GetWherePagedQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>);

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly TRepository Repository;

    /// <summary>
    /// The repository instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="repository">The generic repository</param>
    /// <param name="logger">The logger</param>
    public GetWherePagedQueryHandler(TRepository repository, ILogger<GetWherePagedQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<PagedResult<TEntityDto>> Handle(GetWherePagedQuery<TEntityDto, TEntityDbo, TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Predicate, nameof(request.Predicate));
            Guard.Against.NegativeOrZero(request.PageNumber, nameof(request.PageNumber));
            Guard.Against.NegativeOrZero(request.PageSize, nameof(request.PageSize));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method}. Page number: {pageNumber}, page size: {pageSize}", EntityName, HandlerName, request.PageNumber, request.PageSize);
            }

            var pagedResult = await Repository.WhereAsync(request.Predicate, request.PageNumber, request.PageSize, null, cancellationToken).ConfigureAwait(false);
            return new PagedResult<TEntityDto>
            {
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items.ToList().ConvertAll(MapToDto)
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities by the given predicate: {reason}", EntityName, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
