namespace Modern.Repositories.EFCore.Configuration;

/// <summary>
/// Represents EF Core repository configuration
/// </summary>
public class EfCoreRepositoryConfiguration
{
    /// <summary>
    /// Create statement configuration
    /// </summary>
    public EfCoreExecuteInTransactionConfiguration? CreateConfiguration { get; set; }

    /// <summary>
    /// Update statement configuration
    /// </summary>
    public EfCoreExecuteInTransactionConfiguration? UpdateConfiguration { get; set; }

    /// <summary>
    /// Delete statement configuration
    /// </summary>
    public EfCoreExecuteInTransactionConfiguration? DeleteConfiguration { get; set; }
}