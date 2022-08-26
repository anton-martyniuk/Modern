namespace Modern.Repositories.Abstractions.Infrastracture;

/// <summary>
/// The entity query for selecting nested entities.<br/>
/// Supports nested include queries.
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public class EntityIncludeQuery<TEntity>
{
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _expression;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="expression">Expression that describes included entities</param>
    public EntityIncludeQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> expression)
    {
        _expression = expression;
    }

    /// <summary>
    /// Returns query that combines with expression that describes included entities
    /// </summary>
    /// <param name="query">Query to be wrapped</param>
    /// <returns>Returns <see cref="IQueryable{TEntity}"/> implementation</returns>
    public IQueryable<TEntity> GetExpression(IQueryable<TEntity> query) => _expression(query);
}