using System.Linq.Expressions;

namespace Modern.Repositories.Abstractions.Specifications;

/// <summary>
/// The search specification definition
/// </summary>
/// <typeparam name="TEntity">Type of the entity</typeparam>
public interface ISpecification<TEntity>
    where TEntity : class
{
    /// <summary>
    /// A filtering function to test each element for condition
    /// </summary>
    Expression<Func<TEntity, bool>>? FilterQuery { get; }

    /// <summary>
    /// A collection of functions that describes included entities
    /// </summary>
    IReadOnlyCollection<Expression<Func<TEntity, object>>>? IncludeQueries { get; }

    /// <summary>
    /// A function that describes how to order entities by ascending
    /// </summary>
    IReadOnlyCollection<Expression<Func<TEntity, object>>>? OrderByQueries { get; }

    /// <summary>
    /// A function that describes how to order entities by descending
    /// </summary>
    IReadOnlyCollection<Expression<Func<TEntity, object>>>? OrderByDescendingQueries { get; }
}