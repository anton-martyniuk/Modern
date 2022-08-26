using System.Data;

namespace Modern.Repositories.Dapper.Providers;

/// <summary>
/// The query provider factory definition
/// </summary>
public interface IQueryProviderFactory
{
    /// <summary>
    /// Returns a query provider type based on given database connection
    /// </summary>
    /// <param name="dbConnection">The database connection</param>
    /// <returns>Query provider type</returns>
    IQueryProvider Get(IDbConnection dbConnection);
}
