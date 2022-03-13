using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Modern.Repositories.EFCore.Extensions;

/// <summary>
/// A static class with extension methods for queryable
/// </summary>
internal static class QueryableExtension
{
    /// <summary>
    /// Applies filtering the given <paramref name="query"/> by primary key
    /// </summary>
    /// <typeparam name="TEntity">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of the entity's primary key</typeparam>
    /// <param name="query">Source query</param>
    /// <param name="entityType"><see cref="IEntityType"/></param>
    /// <param name="id">Entity id</param>
    /// <returns>Query with applied filter</returns>
    /// <exception cref="ArgumentException">Thrown if entity does not have any primary key defined</exception>
    public static IQueryable<TEntity> WhereIdEquals<TEntity, TId>(this IQueryable<TEntity> query, IEntityType entityType, TId id)
        where TEntity: class
    {
        var primaryKeyName = entityType.FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault();
        var primaryKeyType = entityType.FindPrimaryKey()?.Properties.Select(p => p.ClrType).FirstOrDefault();

        if (primaryKeyName is null || primaryKeyType is null)
        {
            throw new ArgumentException("Entity does not have any primary key defined", nameof(entityType));
        }

        var pe = Expression.Parameter(typeof(TEntity), "entity");
        var me = Expression.Property(pe, primaryKeyName);
        var constant = Expression.Constant(id, primaryKeyType);
        var body = Expression.Equal(me, constant);
        var expression = Expression.Lambda<Func<TEntity, bool>>(body, pe);

        return query.Where(expression);
    }

    /// <summary>
    /// Applies filtering the given <paramref name="query"/> by primary key
    /// </summary>
    /// <typeparam name="TEntity">The type of entity contained in the data store</typeparam>
    /// <typeparam name="TId">The type of the entity's primary key</typeparam>
    /// <param name="query">Source query</param>
    /// <param name="entityType"><see cref="IEntityType"/></param>
    /// <param name="ids">List of entity ids</param>
    /// <returns>Query with applied filter</returns>
    /// <exception cref="ArgumentException">Thrown if entity does not have any primary key defined</exception>
    public static IQueryable<TEntity> WhereIdEquals<TEntity, TId>(this IQueryable<TEntity> query, IEntityType entityType, List<TId> ids)
        where TEntity : class
    {
        var primaryKeyName = entityType.FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault();
        var primaryKeyType = entityType.FindPrimaryKey()?.Properties.Select(p => p.ClrType).FirstOrDefault();

        if (primaryKeyName is null || primaryKeyType is null)
        {
            throw new ArgumentException("Entity does not have any primary key defined", nameof(entityType));
        }

        var pe = Expression.Parameter(typeof(TEntity), "entity");
        var me = Expression.Property(pe, primaryKeyName);
        var body = Expression.Call(me, typeof(TId).GetMethod("Contains", new[] { typeof(TId) })!, pe);
        var expression = Expression.Lambda<Func<TEntity, bool>>(body, pe);

        return query.Where(expression);
    }
}