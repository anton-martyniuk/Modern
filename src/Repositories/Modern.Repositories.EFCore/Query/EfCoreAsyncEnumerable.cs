using Microsoft.EntityFrameworkCore;

namespace Modern.Repositories.EFCore.Query;

/// <summary>
/// The <see cref="IAsyncEnumerable{T}"/> implementation for EF Core
/// </summary>
internal class EfCoreAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly DbContext _dbContext;
    private readonly IAsyncEnumerable<T> _enumerable;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContext">The database context</param>
    /// <param name="enumerable">The asynchronous enumerable of type <typeparamref name="T"/></param>
    public EfCoreAsyncEnumerable(DbContext dbContext, IAsyncEnumerable<T> enumerable)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));

        _dbContext = dbContext;
        _enumerable = enumerable;
    }

    /// <summary>
    /// <inheritdoc cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/>
    /// </summary>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new EfCoreAsyncEnumerator<T>(_dbContext, _enumerable.GetAsyncEnumerator(cancellationToken));
}