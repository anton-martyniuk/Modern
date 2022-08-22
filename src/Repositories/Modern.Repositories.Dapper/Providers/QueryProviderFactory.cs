using System.Data;

namespace Modern.Repositories.Dapper.Providers;

/// <summary>
/// The <see cref="IQueryProviderFactory"/> implementation
/// </summary>
public class QueryProviderFactory : IQueryProviderFactory
{
    /// <summary>
    /// <inheritdoc cref="IQueryProviderFactory.Get"/>
    /// </summary>
    public IQueryProvider Get(IDbConnection dbConnection)
    {
        var connectionName = dbConnection.GetType().Name;
        return connectionName switch
        {
            "sqlconnection" => new SqlServerQueryProvider(),
            "npgsqlconnection" => new PostgresQueryProvider(),
            "mysqlconnection" => new MySqlQueryProvider(),
            "sqliteconnection" => new SqliteQueryProvider(),
            "oracleconnection" => new OracleQueryProvider(),
            _ => throw new NotSupportedException($"'{connectionName}' database connection provider is not supported")
        };
    }
}
