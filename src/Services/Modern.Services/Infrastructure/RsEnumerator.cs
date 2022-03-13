using System.Collections;

namespace Modern.Services.Infrastructure;

/// <summary>
/// Represents a wrapper around <see cref="IEnumerator{TEntity}"/>
/// </summary>
internal sealed class RsEnumerator<TEntity> : IEnumerator<TEntity>
{
    private readonly IEnumerator<TEntity> _enumerator;
    private bool _disposed;

    object IEnumerator.Current => Current;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="enumerator">The <see cref="IEnumerator{TEntity}"/> to wrap. Field is required</param>
    public RsEnumerator(IEnumerator<TEntity> enumerator)
    {
        _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
    }

    public bool MoveNext()
    {
        EnsureNotDisposed();

        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        EnsureNotDisposed();

        _enumerator.Reset();
    }

    public TEntity Current
    {
        get
        {
            EnsureNotDisposed();

            return _enumerator.Current;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _enumerator.Dispose();
    }

    #region Utils

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name, "Enumerator already disposed");
    }

    #endregion
}