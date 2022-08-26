namespace Modern.Data.Paging;

/// <summary>
/// The paged result model
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// The page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The total number of items in the result set
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total count of items
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Selected items
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}