using System.ComponentModel.DataAnnotations;

namespace Modern.Data.Querying;

/// <summary>
/// Represents a data source query result
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class DataSourceQueryResult<TEntity> where TEntity : class
{
    /// <summary>
    /// The items (records) that match the condition specified in query
    /// </summary>
    public List<TEntity>? Items { get; set; }

    /// <summary>
    /// The total count of items (records) that match the condition specified in query
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Total { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public DataSourceQueryResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="items">The items (records) that match the condition specified in query</param>
    /// <param name="total">The total count of items (records) that match the condition specified in query</param>
    public DataSourceQueryResult(List<TEntity> items, int? total = null)
    {
        Items = items;
        Total = total;
    }
}