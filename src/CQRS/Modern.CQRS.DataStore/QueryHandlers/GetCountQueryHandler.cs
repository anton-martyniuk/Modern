using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstract;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.QueryHandlers;

/// <summary>
/// The mediator query handler that returns the total count of entities that match the given predicate
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
/// <returns>Count of entities</returns>
public class GetCountQueryHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo>,
    IRequestHandler<GetCountQuery<TEntityDbo, TId>, long>

    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(GetCountQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public GetCountQueryHandler(TRepository repository, ILogger<GetCountQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<long> Handle(GetCountQuery<TEntityDbo, TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Predicate, nameof(request.Predicate));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method}", EntityName, HandlerName);
            }

            return await Repository.CountAsync(request.Predicate, null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get {name} entities count by the given predicate: {reason}", EntityName, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
