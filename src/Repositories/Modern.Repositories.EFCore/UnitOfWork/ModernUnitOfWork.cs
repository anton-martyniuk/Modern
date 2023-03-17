using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modern.Exceptions;

namespace Modern.Repositories.EFCore.UnitOfWork;

/// <summary>
/// The <see cref="IModernUnitOfWork"/> implementation
/// </summary>
public class ModernUnitOfWork<TDbContext> : IModernUnitOfWork, IDisposable
    where TDbContext : DbContext
{
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// The <typeparamref name="TDbContext"/> DbContext
    /// </summary>
    protected TDbContext DbContext { get; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/></param>
    public ModernUnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork.SaveChangesAsync"/>
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork.BeginTransactionAsync"/>
    /// </summary>
    public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        _transaction = await DbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        return _transaction;
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork.CommitTransactionAsync"/>
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new RepositoryErrorException("Trying to commit transaction while it hasn't been started");
        }
        
        await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork.RollbackTransactionAsync"/>
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new RepositoryErrorException("Trying to rollback transaction while it hasn't been started");
        }
        
        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
    }
}