using System.Data.Common;

namespace Modern.Repositories.Dapper.Connection;

/// <summary>
/// The database connection factory definition
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns database connection object
    /// </summary>
    /// <returns>Database connection</returns>
    DbConnection CreateDbConnection();
}