using System.ComponentModel.DataAnnotations;

namespace Modern.Data.Querying;

/// <summary>
/// Represents an order by expression model
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class InMemoryOrderByExpression<TEntity> where TEntity : class
{
    /// <summary>
    /// The entity property selector
    /// </summary>
    [Required]
    public Func<TEntity, object> Expression { get; set; }

    /// <summary>
    /// The sorting direction
    /// </summary>
    public SortDirection Direction { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="expression">The entity property selector</param>
    /// <param name="direction">The sorting direction</param>
    public InMemoryOrderByExpression(Func<TEntity, object> expression, SortDirection direction = SortDirection.Asc)
    {
        Expression = expression;
        Direction = direction;
    }
}