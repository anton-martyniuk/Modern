using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;

namespace Modern.Repositories.EFCore.UnitOfWork;

/// <summary>
/// The <see cref="IModernUnitOfWork{TEntity,TId}"/> implementation
/// </summary>
public class ModernUnitOfWork<TDbContext, TEntity, TId> : IModernUnitOfWork<TEntity, TId>, IDisposable
    where TDbContext : DbContext
    where TEntity : class
    where TId : IEquatable<TId>
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
    /// <inheritdoc cref="IModernUnitOfWork{TEntity,TId}.GetRepository"/>
    /// </summary>
    public IModernRepository<TEntity, TId> GetRepository()
        => new ModernEfCoreRepositoryForUnitOfWork<TDbContext, TEntity, TId>(DbContext);

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork{TEntity,TId}.SaveChangesAsync"/>
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork{TEntity,TId}.BeginTransactionAsync"/>
    /// </summary>
    public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        _transaction = await DbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        return _transaction;
    }

    /// <summary>
    /// <inheritdoc cref="IModernUnitOfWork{TEntity,TId}.CommitTransactionAsync"/>
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
    /// <inheritdoc cref="IModernUnitOfWork{TEntity,TId}.RollbackTransactionAsync"/>
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