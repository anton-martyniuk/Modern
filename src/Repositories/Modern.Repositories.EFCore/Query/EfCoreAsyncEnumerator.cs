using Microsoft.EntityFrameworkCore;

namespace Modern.Repositories.EFCore.Query;

/// <summary>
/// The <see cref="IAsyncEnumerator{T}"/> implementation for EF Core
/// </summary>
internal sealed class EfCoreAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly DbContext _dbContext;
    private readonly IAsyncEnumerator<T> _enumerator;

    /// <summary>
    /// <inheritdoc cref="IAsyncEnumerator{T}.Current"/>
    /// </summary>
    public T Current => _enumerator.Current;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContext">The database context</param>
    /// <param name="enumerator">The asynchronous enumerator of type <typeparamref name="T"/></param>
    public EfCoreAsyncEnumerator(DbContext dbContext, IAsyncEnumerator<T> enumerator)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(enumerator, nameof(enumerator));

        _dbContext = dbContext;
        _enumerator = enumerator;
    }

    /// <summary>
    /// <inheritdoc cref="IAsyncEnumerator{T}.MoveNextAsync"/>
    /// </summary>
    public ValueTask<bool> MoveNextAsync() => _enumerator.MoveNextAsync();

    /// <summary>
    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _enumerator.DisposeAsync().ConfigureAwait(false);
        await _dbContext.DisposeAsync().ConfigureAwait(false);
    }
}