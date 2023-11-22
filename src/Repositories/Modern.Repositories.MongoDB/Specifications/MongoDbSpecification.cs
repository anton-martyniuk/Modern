using Modern.Repositories.Abstractions.Specifications;
using Modern.Repositories.MongoDB.Specifications.Base;
using MongoDB.Driver.Linq;

namespace Modern.Repositories.MongoDB.Specifications;

/// <summary>
/// The MongoDb specification implementation
/// </summary>
/// <typeparam name="TEntity">Type of the entity</typeparam>
public class MongoDbSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public MongoDbSpecification(ISpecification<TEntity> specification) : base(specification)
    {
    }

    /// <summary>
    /// Returns a specification expression.<br/>
    /// IMPORTANT: MongoDb Specification doesn't support IncludeQueries
    /// </summary>
    /// <returns>Expression</returns>
    public virtual IMongoQueryable<TEntity> Apply(IMongoQueryable<TEntity> queryable)
    {
        if (FilterQuery is not null)
        {
            queryable = queryable.Where(FilterQuery);
        }

        // IncludeQueries are not supported

        if (OrderByQueries?.Count > 0)
        {
            var orderedQueryable = queryable.OrderBy(OrderByQueries.First());
            orderedQueryable = OrderByQueries.Skip(1).Aggregate(orderedQueryable, (current, orderQuery) => current.ThenBy(orderQuery));
            queryable = orderedQueryable;
        }
        
        if (OrderByDescendingQueries?.Count > 0)
        {
            var orderedQueryable = queryable.OrderByDescending(OrderByDescendingQueries.First());
            orderedQueryable = OrderByDescendingQueries.Skip(1).Aggregate(orderedQueryable, (current, orderQuery) => current.ThenByDescending(orderQuery));
            queryable = orderedQueryable;
        }

        return queryable;
    }
}