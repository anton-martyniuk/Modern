using System.Data;

namespace Modern.Repositories.EFCore.Configuration;

/// <summary>
/// Represents a transaction configuration
/// </summary>
public class EfCoreExecuteInTransactionConfiguration
{
    /// <summary>
    /// Specifies whether a statement should be executed in a EF Core transaction
    /// </summary>
    public bool NeedExecuteInTransaction { get; set; }

    /// <summary>
    /// Specifies an isolation level of the transaction
    /// </summary>
    public IsolationLevel TransactionIsolationLevel { get; set; } = IsolationLevel.Unspecified;
}