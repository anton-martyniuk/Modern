using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Modern.Repositories.Abstractions;

namespace Modern.Repositories.EFCore.UnitOfWork;

/// <summary>
/// The modern unit of work definition
/// </summary>
public interface IModernUnitOfWork<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Returns a repository that shares the same DbContext as UnitOfWork
    /// </summary>
    /// <returns>Modern repository definition</returns>
    IModernRepository<TEntity, TId> GetRepository();
    
    /// <summary>
    /// Performs saving changes in the DbContext
    /// </summary>
    /// <returns>Number of rows affected</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Creates and starts the database transaction
    /// </summary>
    /// <param name="isolationLevel">Isolation level of transaction</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>A transaction against the database</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all changes made to the database in the current transaction
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Discards all changes made to the database in the current transaction
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}