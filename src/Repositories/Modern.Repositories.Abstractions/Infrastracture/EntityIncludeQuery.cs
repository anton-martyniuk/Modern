namespace Modern.Repositories.Abstractions.Infrastracture;

/// <summary>
/// The entity query for selecting nested entities.<br/>
/// Supports nested include queries.
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public class EntityIncludeQuery<TEntity>
{
    /// <summary>
    /// Expression that describes included entities
    /// </summary>
    public Func<IQueryable<TEntity>, IQueryable<TEntity>> Expression { get; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="expression">Expression that describes included entities</param>
    public EntityIncludeQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> expression)
    {
        Expression = expression;
    }
}