using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Modern.Exceptions;

namespace Modern.Repositories.EFCore.Query;

/// <summary>
/// The <see cref="IQueryProvider"/> implementation.<br/>
/// Defines custom query provider with EF Core implementation over standard <see cref="IDbQueryProvider"/> for the database queries
/// using <see cref="IDbContextFactory{TContext}"/>
/// </summary>
internal sealed class EfCoreQueryProviderWithFactory<TDbContext, TEntity> : IAsyncQueryProvider, IDbQueryProvider
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly IDbContextFactory<TDbContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContextFactory">The <see cref="IDbContextFactory{TDbContext}"/> implementation</param>
    public EfCoreQueryProviderWithFactory(IDbContextFactory<TDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }

    /// <summary>
    /// <inheritdoc cref="IQueryProvider.CreateQuery"/>
    /// </summary>
    public IQueryable CreateQuery(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        Type? expressionType = null;

        try
        {
            expressionType = expression.Type.GetGenericArguments().First();

            return (IQueryable)(Activator.CreateInstance(typeof(EfCoreQueryable<>)
                .MakeGenericType(expressionType), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { this, expression }, null, null)
                ?? throw new InvalidOperationException("Converted expression returned null"));
        }
        catch (Exception ex)
        {
            throw new RepositoryErrorException($"Unable to create instance of 'Queryable<{(expressionType ?? expression.Type).Name}>'. {ex.Message}", ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IQueryProvider.CreateQuery{TElement}"/>
    /// </summary>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        try
        {
            return new EfCoreQueryable<TElement>(this, expression);
        }
        catch (Exception ex)
        {
            throw new RepositoryErrorException($"Unable to create instance of '{typeof(EfCoreQueryable<TElement>).Name}'. {ex.Message}", ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IQueryProvider.Execute"/>
    /// </summary>
    public object Execute(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        try
        {
            using var db = _dbContextFactory.CreateDbContext();
            var queryable = db.Set<TEntity>().AsNoTracking();
            var convertedExpression = ChangeQueryableInExpression(expression, queryable);

            return queryable.Provider.Execute(convertedExpression) ?? throw new InvalidOperationException("Converted expression returned null");
        }
        catch (Exception ex)
        {
            throw new RepositoryErrorException($"Unable to query data. {ex.Message}", ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IQueryProvider.Execute{TResult}"/>
    /// </summary>
    public TResult Execute<TResult>(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        try
        {
            using var db = _dbContextFactory.CreateDbContext();
            var queryable = db.Set<TEntity>().AsNoTracking();
            var convertedExpression = ChangeQueryableInExpression(expression, queryable);

            return queryable.Provider.Execute<TResult>(convertedExpression);
        }
        catch (Exception ex)
        {
            throw new RepositoryErrorException($"Unable to query data. {ex.Message}", ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IAsyncQueryProvider.ExecuteAsync{TResult}"/>
    /// </summary>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var result = Execute(expression);

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { result })!;
    }

    /// <summary>
    /// <inheritdoc cref="IDbQueryProvider.GetEnumerator{T}"/>
    /// </summary>
    public IEnumerator<T> GetEnumerator<T>(Expression expression)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var queryable = db.Set<TEntity>().AsNoTracking();
        var convertedExpression = ChangeQueryableInExpression(expression, queryable);

        foreach (var obj in queryable.Provider.Execute<IEnumerable<T>>(convertedExpression))
        {
            yield return obj;
        }
    }

    /// <summary>
    /// <inheritdoc cref="IDbQueryProvider.GetEnumerator"/>
    /// </summary>
    public IEnumerator GetEnumerator(Expression expression)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var queryable = db.Set<TEntity>().AsNoTracking();
        var convertedExpression = ChangeQueryableInExpression(expression, queryable);

        foreach (var obj in queryable.Provider.Execute<IEnumerable>(convertedExpression).Cast<object>())
        {
            yield return obj;
        }
    }

    /// <summary>
    /// Converts the <see cref="IQueryable{T}"/> source of the given <paramref name="expression"/> from <see cref="EfCoreQueryable{T}"/> to the specified <paramref name="queryable"/>.<br/>
    /// For example, converts <see cref="EfCoreQueryable{T}"/> expression to the database IQueryable
    /// </summary>
    private static Expression ChangeQueryableInExpression(Expression expression, IQueryable<TEntity> queryable)
    {
        var visitor = new ExpressionQueryableSourceModifierVisitor(queryable.Expression);
        return visitor.Visit(expression);
    }

    /// <summary>
    /// Represents a <see cref="ExpressionVisitor"/> for replacing expression from <see cref="EfCoreQueryable{T}"/> to the given expression
    /// </summary>
    private sealed class ExpressionQueryableSourceModifierVisitor : ExpressionVisitor
    {
        private readonly Expression _expression;
        private static readonly Type QueryableTypeToReplace = typeof(EfCoreQueryable<TEntity>);

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="expression">Expression to replace with</param>
        public ExpressionQueryableSourceModifierVisitor(Expression expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// <inheritdoc cref="ExpressionVisitor.VisitConstant"/>
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression node)
            => node.Type == QueryableTypeToReplace ? _expression : base.VisitConstant(node);
    }
}