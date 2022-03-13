using System.Collections;

namespace Modern.Services.Infrastructure;

/// <summary>
/// Represents a wrapper around <see cref="IEnumerable{TEntity}"/> with support of stopping enumeration after service stop/dispose
/// </summary>
internal sealed class RsEnumerable<TEntity> : IEnumerable<TEntity>
{
    private readonly IEnumerable<TEntity> _enumerable;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="enumerable">The <see cref="IEnumerable{TEntity}"/> to wrap. Field is required</param>
    public RsEnumerable(IEnumerable<TEntity> enumerable)
    {
        _enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
    }

    /// <inheritdoc />
    public IEnumerator<TEntity> GetEnumerator()
    {
        return new RsEnumerator<TEntity>(_enumerable.GetEnumerator());
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}