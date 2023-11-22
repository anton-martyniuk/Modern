using System.Data;
using Dapper;

namespace Modern.Repositories.Dapper.Extensions;

/// <summary>
/// Async extension for Dapper
/// </summary>
internal static class DapperAsyncExtensions
{
    /// <summary>
    /// Executes sql command
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of executing command</returns>
    public static async Task<int> ExecuteWithTokenAsync(this IDbConnection connection,
        string sql,
        object param = null!,
        IDbTransaction transaction = null!,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.ExecuteAsync(
            new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Executes sql command to query entities
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of queried entities</returns>
    public static async Task<IEnumerable<T>> QueryWithTokenAsync<T>(this IDbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Executes sql command and returns a single entity
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity that matches the given sql query or <see langword="null"/> if entity not found</returns>
    public static async Task<T> QuerySingleWithTokenAsync<T>(this IDbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.QuerySingleAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Executes sql command to get a single entity
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity that matches the given sql query or <see langword="null"/> if entity not found</returns>
    public static async Task<T?> QuerySingleOrDefaultWithTokenAsync<T>(this IDbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Executes sql command to get a single entity
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity that matches the given sql query</returns>
    public static async Task<T> QueryFirstWithTokenAsync<T>(this IDbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.QueryFirstAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Executes sql command to get a single entity
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="sql">Sql script</param>
    /// <param name="param">Object parameter</param>
    /// <param name="transaction">Database transaction</param>
    /// <param name="commandTimeout">Timeout ot execute sql command</param>
    /// <param name="commandType">Type of command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity that matches the given sql query or <see langword="null"/> if entity not found</returns>
    public static async Task<T?> QueryFirstOrDefaultWithTokenAsync<T>(this IDbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType,
            cancellationToken: cancellationToken));
    }
}