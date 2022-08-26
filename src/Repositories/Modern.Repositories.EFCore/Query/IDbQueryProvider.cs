using System.Collections;
using System.Linq.Expressions;

namespace Modern.Repositories.EFCore.Query;

/// <summary>
/// The query provider definition.<br/>
/// Defines custom query provider interface over standard <see cref="IDbQueryProvider"/> for the database queries
/// </summary>
internal interface IDbQueryProvider : IQueryProvider
{
    /// <summary>
    /// Returns Enumerator of <typeparamref name="T"/> for given expression
    /// </summary>
    /// <typeparam name="T">Type of object which is being queried</typeparam>
    /// <param name="expression">Expression object with rules to be applied to the queryable object</param>
    /// <returns><see cref="IEnumerator"/> of type <typeparamref name="T"/></returns>
    IEnumerator<T> GetEnumerator<T>(Expression expression);

    /// <summary>
    /// Returns Enumerator of object type for given expression
    /// </summary>
    /// <param name="expression">Expression object with rules to be applied to the queryable object</param>
    /// <returns><see cref="IEnumerator"/> of object type</returns>
    IEnumerator GetEnumerator(Expression expression);
}