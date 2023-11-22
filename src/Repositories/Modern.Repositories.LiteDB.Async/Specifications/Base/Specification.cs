using System.Linq.Expressions;
using Modern.Repositories.Abstractions.Specifications;

namespace Modern.Repositories.LiteDB.Async.Specifications.Base;

/// <summary>
/// The search specification definition
/// </summary>
/// <typeparam name="TEntity">Type of the entity</typeparam>
public abstract class Specification<TEntity> : ISpecification<TEntity>
    where TEntity : class
{
    private List<Expression<Func<TEntity, object>>>? _includeQueries;
    private List<Expression<Func<TEntity, object>>>? _orderByQueries;
    private List<Expression<Func<TEntity, object>>>? _orderByDescendingQueries;
    
    /// <summary>
    /// A filtering function to test each element for condition
    /// </summary>
    public Expression<Func<TEntity, bool>>? FilterQuery { get; private set; }

    /// <summary>
    /// A collection of functions that describes included entities
    /// </summary>
    public IReadOnlyCollection<Expression<Func<TEntity, object>>>? IncludeQueries => _includeQueries;

    /// <summary>
    /// A function that describes how to order entities by ascending
    /// </summary>
    public IReadOnlyCollection<Expression<Func<TEntity, object>>>? OrderByQueries => _orderByQueries;

    /// <summary>
    /// A function that describes how to order entities by descending
    /// </summary>
    public IReadOnlyCollection<Expression<Func<TEntity, object>>>? OrderByDescendingQueries => _orderByDescendingQueries;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    protected Specification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="query">A filtering function to test each element for condition</param>
    protected Specification(Expression<Func<TEntity, bool>> query)
    {
        FilterQuery = query;
    }
    
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="specification">A specification to be built</param>
    protected Specification(ISpecification<TEntity> specification)
    {
        FilterQuery = specification.FilterQuery;

        _includeQueries = specification.IncludeQueries?.ToList();
        _orderByQueries = specification.OrderByQueries?.ToList();
        _orderByDescendingQueries = specification.OrderByDescendingQueries?.ToList();
    }

    /// <summary>
    /// Adds a filtering function to test each element for condition
    /// </summary>
    /// <param name="query">A filtering function that describes how to test each element for condition</param>
    protected void AddFilteringQuery(Expression<Func<TEntity, bool>> query)
    {
        FilterQuery = query;
    }

    /// <summary>
    /// Adds an query that describes included entities
    /// </summary>
    /// <param name="query">Expression that describes included entities</param>
    protected void AddIncludeQuery(Expression<Func<TEntity, object>> query)
    {
        _includeQueries ??= new();
        _includeQueries.Add(query);
    }
    
    /// <summary>
    /// Adds a query that orders entities by ascending
    /// </summary>
    /// <param name="query">A function that describes how to order entities by ascending</param>
    protected void AddOrderByQuery(Expression<Func<TEntity, object>> query)
    {
        _orderByQueries ??= new();
        _orderByQueries.Add(query);
    }
    
    /// <summary>
    /// Adds a query that orders entities by descending
    /// </summary>
    /// <param name="query">A function that describes how to order entities by descending</param>
    protected void AddOrderByDescendingQuery(Expression<Func<TEntity, object>> query)
    {
        _orderByDescendingQueries ??= new();
        _orderByDescendingQueries.Add(query);
    }
    
    /// <summary>
    /// Returns an indication whether an entity matches the current specification
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns><see langword="true"/> if an entity matches the current specification; otherwise, <see langword="false"/></returns>
    public bool Match(TEntity entity)
    {
        if (FilterQuery is null)
        {
            return false;
        }
        
        var predicate = FilterQuery.Compile();
        return predicate(entity);
    }

    /// <summary>
    /// Returns an expression that combines two specifications with a logical "and"
    /// </summary>
    /// <param name="specification">Specification to combine with</param>
    /// <returns><see cref="AndSpecification{T}"/></returns>
    public Specification<TEntity> And(Specification<TEntity> specification)
        => new AndSpecification<TEntity>(this, specification);

    /// <summary>
    /// Returns an expression that combines two specifications with a logical "or"
    /// </summary>
    /// <param name="specification">Specification to combine with</param>
    /// <returns><see cref="OrSpecification{T}"/></returns>
    public Specification<TEntity> Or(Specification<TEntity> specification)
        => new OrSpecification<TEntity>(this, specification);
}