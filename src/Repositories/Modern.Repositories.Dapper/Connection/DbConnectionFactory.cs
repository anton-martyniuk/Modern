using System.Data.Common;

namespace Modern.Repositories.Dapper.Connection;

/// <summary>
/// The <see cref="IDbConnectionFactory"/> implementation
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly Func<DbConnection> _createDbConnection;
    
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="createDbConnection">A function that creates a database connection</param>
    public DbConnectionFactory(Func<DbConnection> createDbConnection)
    {
        _createDbConnection = createDbConnection;
    }

    /// <summary>
    /// <inheritdoc cref="IDbConnectionFactory.CreateDbConnection"/>
    /// </summary>
    public DbConnection CreateDbConnection() => _createDbConnection();
}