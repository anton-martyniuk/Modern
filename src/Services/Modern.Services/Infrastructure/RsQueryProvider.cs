using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Modern.Services.Infrastructure;

/// <summary>
/// Represents a wrapper around <see cref="IQueryProvider"/>
/// </summary>
internal sealed class RsQueryProvider : IAsyncQueryProvider
{
    private readonly IQueryProvider _queryProvider;

    /// <summary>
    /// The logger
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="queryProvider">QueryProvider</param>
    /// <param name="logger">Logger (not required)</param>
    public RsQueryProvider(IQueryProvider queryProvider, ILogger logger = null)
    {
        _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
        Logger = logger;
    }

    /// <inheritdoc/>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        try
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            var queryable = _queryProvider.CreateQuery<TElement>(expression);
            return new RsQueryable<TElement>(queryable, this);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to create instance of '{typeof(RsQueryable<TElement>).Name}'. {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public IQueryable CreateQuery(Expression expression)
    {
        Type elementType = null;
        try
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            var queryable = _queryProvider.CreateQuery(expression);
            elementType = expression.Type.GetGenericArguments().First();
            return (IQueryable)Activator.CreateInstance(typeof(RsQueryable<>).MakeGenericType(elementType), queryable, this);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to create instance of 'RsQueryable<{(elementType ?? expression.Type).Name}>'. {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public object Execute(Expression expression)
    {
        try
        {
            if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                Logger?.LogTrace("Performing query over IQueryable... {0}", expression);
            }

            return _queryProvider.Execute(expression);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unable to perform query over IQueryable {0}", expression);
            throw;
        }
    }

    /// <inheritdoc />
    public TResult Execute<TResult>(Expression expression)
    {
        try
        {
            if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                Logger?.LogTrace("Performing query over IQueryable... {0}", expression);
            }

            return _queryProvider.Execute<TResult>(expression);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unable to perform query over IQueryable {0}", expression);
            throw;
        }
    }

    /// <inheritdoc />
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        try
        {
            if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                Logger?.LogTrace("Performing query over IQueryable... {0}", expression);
            }

            var method = _queryProvider.GetType().GetMethod("ExecuteAsync");
            if (method is null) throw new NotSupportedException("Asynchronous execution is not supported");
            var generic = method.MakeGenericMethod(typeof(TResult));
            return (TResult)generic.Invoke(_queryProvider, new object[] { expression, cancellationToken });
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unable to perform query over IQueryable {0}", expression);
            throw;
        }
    }
}