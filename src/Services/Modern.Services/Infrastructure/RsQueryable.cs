using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq.Expressions;

namespace Modern.Services.Infrastructure;

/// <summary>
/// Represents a wrapper around <see cref="IQueryable{T}"/>
/// </summary>
internal sealed class RsQueryable<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
{
    private readonly IQueryable<T> _sourceQueryable;
    private readonly RsQueryProvider _provider;

    /// <inheritdoc/>
    public Type ElementType => _sourceQueryable.ElementType;

    /// <inheritdoc/>
    public Expression Expression => _sourceQueryable.Expression;

    /// <inheritdoc/>
    public IQueryProvider Provider => _provider;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="sourceQueryable">The original queryable. Usually repository queryable</param>
    /// <param name="logger">Logger (not required)</param>
    public RsQueryable(IQueryable<T> sourceQueryable, ILogger logger = null)
    {
        _sourceQueryable = sourceQueryable ?? throw new ArgumentNullException(nameof(sourceQueryable));
        _provider = new RsQueryProvider(sourceQueryable.Provider, logger);
    }

    /// <summary>
    /// Constructor for creating instances from <see cref="RsQueryProvider"/>. Do not use this constructor in service directly
    /// </summary>
    /// <param name="sourceQueryable">The original queryable. Usually repository queryable</param>
    /// <param name="provider">The query provider</param>
    public RsQueryable(IQueryable<T> sourceQueryable, RsQueryProvider provider)
    {
        _sourceQueryable = sourceQueryable ?? throw new ArgumentNullException(nameof(sourceQueryable));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        try
        {
            if (_provider.Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                _provider.Logger?.LogTrace("Performing query over IQueryable... {0}", Expression);
            }

            var enumerator = _sourceQueryable.GetEnumerator();
            return new RsEnumerator<T>(enumerator);
        }
        catch (Exception ex)
        {
            _provider.Logger?.LogError(ex, "Unable to perform query over IQueryable {0}", Expression);
            throw;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        try
        {
            if (_provider.Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                _provider.Logger?.LogTrace("Performing query over IQueryable... {0}", Expression);
            }

            var enumerator = ((IQueryable)_sourceQueryable).GetEnumerator();
            return new RsEnumerator<object>((IEnumerator<object>)enumerator);
        }
        catch (Exception ex)
        {
            _provider.Logger?.LogError(ex, "Unable to perform query over IQueryable {0}", Expression);
            throw;
        }
    }

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => _provider
            .ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellationToken)
            .GetAsyncEnumerator(cancellationToken);
}