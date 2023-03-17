using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Modern.Repositories.EFCore.UnitOfWork;

/// <summary>
/// The modern unit of work definition
/// </summary>
public interface IModernUnitOfWork
{
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