﻿using LiteDB;
using Modern.Repositories.Abstractions.Specifications;
using Modern.Repositories.LiteDB.Specifications.Base;

namespace Modern.Repositories.LiteDB.Specifications;

/// <summary>
/// The LiteDb specification implementation
/// </summary>
/// <typeparam name="TEntity">Type of the entity</typeparam>
public class LiteDbSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public LiteDbSpecification(ISpecification<TEntity> specification) : base(specification)
    {
    }

    /// <summary>
    /// Returns a specification expression.<br/>
    /// IMPORTANT: LiteDb Specification supports IncludeQueries
    /// </summary>
    /// <returns>Expression</returns>
    public virtual ILiteQueryable<TEntity> Apply(ILiteQueryable<TEntity> queryable)
    {
        if (FilterQuery is not null)
        {
            queryable = queryable.Where(FilterQuery);
        }
        
        if (IncludeQueries?.Count > 0)
        {
            queryable = IncludeQueries.Aggregate(queryable, (current, includeQuery) => current.Include(includeQuery));
        }

        // LiteDb doesn't not support multiple OrderBy clauses.
        // It is a planned feature for LiteDb v5.1, see more here: https://github.com/mbdavid/LiteDB/issues/1409#issuecomment-576828513
        
        if (OrderByQueries?.Count > 0)
        {
            queryable = queryable.OrderBy(OrderByQueries.First());
        }
        
        if (OrderByDescendingQueries?.Count > 0)
        {
            queryable = queryable.OrderByDescending(OrderByDescendingQueries.First());
        }

        return queryable;
    }
}