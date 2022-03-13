using System.Linq.Expressions;
using Ardalis.GuardClauses;

namespace Modern.Data.Querying;

/// <summary>
/// Represents a fluent builder over <see cref="DataSourceQuery{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class DataSourceQueryBuilder<TEntity> where TEntity : class
{
    private readonly DataSourceQuery<TEntity> _query;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="query">The query instance to configure</param>
    public DataSourceQueryBuilder(DataSourceQuery<TEntity> query)
    {
        _query = query;
    }

    /// <summary>
    /// Sets a <see cref="DataSourceQuery{TEntity}.Where"/> field
    /// </summary>
    /// <param name="expression">The filtering expression</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> expression)
    {
        _query.Where = expression;
        return this;
    }

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (ascending) to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> expression) => OrderBy(expression, SortDirection.Asc);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <param name="direction">The sorting direction</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> expression, SortDirection direction)
    {
        _query.OrderBy?.Add(new OrderByExpression<TEntity>(expression, direction));
        return this;
    }

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (descending) to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        => OrderBy(expression, SortDirection.Desc);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (ascending) to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> ThenBy(Expression<Func<TEntity, object>> expression)
        => OrderBy(expression);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <param name="direction">The sorting direction</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> ThenBy(Expression<Func<TEntity, object>> expression, SortDirection direction)
        => OrderBy(expression, direction);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (descending) to <see cref="DataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> ThenByDescending(Expression<Func<TEntity, object>> expression)
        => OrderByDescending(expression);

    /// <summary>
    /// Sets a number of items to take
    /// </summary>
    /// <param name="take">The number of items to take</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> Take(int take)
    {
        Guard.Against.Negative(take, nameof(take));

        _query.Take = take;
        return this;
    }

    /// <summary>
    /// Sets a number of items to skip
    /// </summary>
    /// <param name="skip">The number of items to skip</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> Skip(int skip)
    {
        Guard.Against.Negative(skip, nameof(skip));

        _query.Skip = skip;
        return this;
    }

    /// <summary>
    /// Specifies whether to include the total count of items (records) that match the specified <see cref="DataSourceQuery{TEntity}.Where"/> condition
    /// </summary>
    /// <param name="includeTotal">Specifies whether to include the total count of items (records) that match the specified <see cref="DataSourceQuery{TEntity}.Where"/> condition</param>
    /// <returns>The current builder instance (this)</returns>
    public DataSourceQueryBuilder<TEntity> IncludeTotal(bool includeTotal = true)
    {
        _query.IncludeTotal = includeTotal;
        return this;
    }
}