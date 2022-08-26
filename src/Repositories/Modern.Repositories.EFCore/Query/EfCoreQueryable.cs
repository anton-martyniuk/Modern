using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Modern.Repositories.EFCore.Query;

/// <summary>
/// Represents a decorator over <see cref="IQueryable{T}"/> for EF Core
/// </summary>
internal sealed class EfCoreQueryable<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
{
    private static readonly Type Type = typeof(T);
    private readonly IDbQueryProvider _provider;

    /// <summary>
    /// <inheritdoc cref="IQueryable.Expression"/>
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// <inheritdoc cref="IQueryable.ElementType"/>
    /// </summary>
    public Type ElementType => Type;

    /// <summary>
    /// <inheritdoc cref="IQueryable.Provider"/>
    /// </summary>
    public IQueryProvider Provider => _provider;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="queryProvider">Query provider</param>
    public EfCoreQueryable(IDbQueryProvider queryProvider)
    {
        _provider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
        Expression = Expression.Constant(this);
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="queryProvider">Query provider</param>
    /// <param name="expression">Expression object with rules to be applied to the queryable object</param>
    internal EfCoreQueryable(IDbQueryProvider queryProvider, Expression expression)
    {
        _provider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    /// <summary>
    /// <inheritdoc cref="IDbQueryProvider.GetEnumerator{T}"/>
    /// </summary>
    public IEnumerator<T> GetEnumerator() => _provider.GetEnumerator<T>(Expression);

    /// <summary>
    /// <inheritdoc cref="IDbQueryProvider.GetEnumerator{T}"/>
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => _provider.GetEnumerator(Expression);

    /// <summary>
    /// <inheritdoc cref="EfCoreQueryProvider{TDbContext,TEntity}.ExecuteAsync{TResult}"/>
    /// </summary>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => ((IAsyncQueryProvider)_provider).ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellationToken).GetAsyncEnumerator(cancellationToken);
}