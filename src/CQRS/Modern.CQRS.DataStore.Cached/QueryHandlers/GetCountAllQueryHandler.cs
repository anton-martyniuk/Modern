using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.CQRS.DataStore.Cached.Abstract;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.Cached.QueryHandlers;

/// <summary>
/// The mediator query handler that returns the total count of entities
/// </summary>
/// <returns>Count of entities</returns>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
public class GetCountAllQueryHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo, TId>,
    IRequestHandler<GetCountAllQuery<TEntityDto, TId>, int>

    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(GetCountAllQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public GetCountAllQueryHandler(TRepository repository, ILogger<GetCountAllQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<int> Handle(GetCountAllQuery<TEntityDto, TId> request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            cancellationToken.ThrowIfCancellationRequested();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("{serviceName}.{method} of all entities", EntityName, HandlerName);
            }

            return await Repository.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get count of all {name} entities: {reason}", EntityName, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
