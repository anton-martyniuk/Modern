using Ardalis.GuardClauses;

namespace Modern.Data.Querying;

/// <summary>
/// Represents a fluent builder over <see cref="InMemoryDataSourceQuery{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class InMemoryDataSourceQueryBuilder<TEntity> where TEntity : class
{
    private readonly InMemoryDataSourceQuery<TEntity> _query;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="query">The query instance to configure</param>
    public InMemoryDataSourceQueryBuilder(InMemoryDataSourceQuery<TEntity> query)
    {
        _query = query;
    }

    /// <summary>
    /// Sets a <see cref="InMemoryDataSourceQuery{TEntity}.Where"/> field
    /// </summary>
    /// <param name="predicate">The filtering predicate</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> Where(Func<TEntity, bool> predicate)
    {
        _query.Where = predicate;
        return this;
    }

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (ascending) to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> OrderBy(Func<TEntity, object> expression) => OrderBy(expression, SortDirection.Asc);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <param name="direction">The sorting direction</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> OrderBy(Func<TEntity, object> expression, SortDirection direction)
    {
        _query.OrderBy.Add(new InMemoryOrderByExpression<TEntity>(expression, direction));
        return this;
    }

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (descending) to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> OrderByDescending(Func<TEntity, object> expression)
        => OrderBy(expression, SortDirection.Desc);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (ascending) to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> ThenBy(Func<TEntity, object> expression)
        => OrderBy(expression);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <param name="direction">The sorting direction</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> ThenBy(Func<TEntity, object> expression, SortDirection direction)
        => OrderBy(expression, direction);

    /// <summary>
    /// Adds an order by <paramref name="expression"/> (descending) to <see cref="InMemoryDataSourceQuery{TEntity}.OrderBy"/> field
    /// </summary>
    /// <param name="expression">The property select expression</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> ThenByDescending(Func<TEntity, object> expression)
        => OrderByDescending(expression);

    /// <summary>
    /// Sets a number of items to take
    /// </summary>
    /// <param name="take">The number of items to take</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> Take(int take)
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
    public InMemoryDataSourceQueryBuilder<TEntity> Skip(int skip)
    {
        Guard.Against.Negative(skip, nameof(skip));

        _query.Skip = skip;
        return this;
    }

    /// <summary>
    /// Specifies whether to include the total count of items (records) that match the specified <see cref="InMemoryDataSourceQuery{TEntity}.Where"/> condition
    /// </summary>
    /// <param name="includeTotal">Specifies whether to include the total count of items (records) that match the specified <see cref="InMemoryDataSourceQuery{TEntity}.Where"/> condition</param>
    /// <returns>The current builder instance (this)</returns>
    public InMemoryDataSourceQueryBuilder<TEntity> IncludeTotal(bool includeTotal = true)
    {
        _query.IncludeTotal = includeTotal;
        return this;
    }
}