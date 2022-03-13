using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Modern.Data.Querying;

/// <summary>
/// Represents an order by expression model
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public sealed class OrderByExpression<TEntity> where TEntity : class
{
    /// <summary>
    /// The entity property selector
    /// </summary>
    [Required]
    public Expression<Func<TEntity, object>> Expression { get; set; }

    /// <summary>
    /// The sorting direction
    /// </summary>
    public SortDirection Direction { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="expression">The entity property selector</param>
    /// <param name="direction">The sorting direction</param>
    public OrderByExpression(Expression<Func<TEntity, object>> expression, SortDirection direction = SortDirection.Asc)
    {
        Expression = expression;
        Direction = direction;
    }
}