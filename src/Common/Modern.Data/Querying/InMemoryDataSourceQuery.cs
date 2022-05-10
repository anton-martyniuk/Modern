using System.ComponentModel.DataAnnotations;

namespace Modern.Data.Querying;

/// <summary>
/// Represents a data source query model
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class InMemoryDataSourceQuery<TEntity> where TEntity : class
{
    /// <summary>
    /// A function to test each element for condition
    /// </summary>
    public Func<TEntity, bool> Where { get; set; }

    /// <summary>
    /// The collection of sorting (order by) expressions
    /// </summary>
    public List<InMemoryOrderByExpression<TEntity>> OrderBy { get; set; }

    /// <summary>
    /// The number of items to take
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Take { get; set; }

    /// <summary>
    /// The number of items to skip
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }

    /// <summary>
    /// Specifies whether to include the total count of items (records) that match the specified <see cref="Where"/> condition
    /// </summary>
    public bool IncludeTotal { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="where">The filtering expression</param>
    /// <param name="orderBy">The collection of sorting (order by) expressions</param>
    /// <param name="take">The number of items to take</param>
    /// <param name="skip">The number of items to skip</param>
    /// <param name="includeTotal">Specifies whether to include the total count of items (records) that match the specified <paramref name="where"/> condition</param>
    public InMemoryDataSourceQuery(Func<TEntity, bool> where, List<InMemoryOrderByExpression<TEntity>> orderBy, int take, int skip, bool includeTotal)
    {
        Where = where;
        OrderBy = orderBy;
        Take = take;
        Skip = skip;
        IncludeTotal = includeTotal;
    }
}