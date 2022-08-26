using MediatR;
using Microsoft.Extensions.Logging;
using Modern.CQRS.DataStore.Abstract;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.CQRS.DataStore.QueryHandlers;

/// <summary>
/// The mediator query handler that returns the single entity that matches the given predicate
/// </summary>
/// <exception cref="ArgumentNullException">Thrown if provided predicate is null</exception>
/// <exception cref="InternalErrorException">Thrown if an error occurred while retrieving entities</exception>
/// <exception cref="InvalidOperationException">Thrown if the data store contains more than one entity that matches the condition</exception>
/// <returns>Entity that matches the given predicate or <see langword="null"/> if entity not found</returns>
public class GetSingleOrDefaultQueryHandler<TEntityDto, TEntityDbo, TId, TRepository> :
    BaseMediatorHandler<TEntityDto, TEntityDbo>,
    IRequestHandler<GetSingleOrDefaultQuery<TEntityDto, TEntityDbo, TId>, TEntityDto?>

    where TEntityDto : class
    where TEntityDbo : class
    where TId : IEquatable<TId>
    where TRepository : class, IModernQueryRepository<TEntityDbo, TId>
{
    private const string HandlerName = nameof(GetSingleOrDefaultQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>);

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
    public GetSingleOrDefaultQueryHandler(TRepository repository, ILogger<GetSingleOrDefaultQueryHandler<TEntityDto, TEntityDbo, TId, TRepository>> logger)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle"/>
    /// </summary>
    public async Task<TEntityDto?> Handle(GetSingleOrDefaultQuery<TEntityDto, TEntityDbo, TId> request, CancellationToken cancellationToken)
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

            var entityDbo = await Repository.SingleOrDefaultAsync(request.Predicate, null, cancellationToken).ConfigureAwait(false);
            return entityDbo is not null ? MapToDto(entityDbo) : null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not get single {name} entity by the given predicate: {reason}", EntityName, ex.Message);
            throw CreateProperException(ex);
        }
    }
}
