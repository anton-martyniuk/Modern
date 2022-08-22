using Modern.Repositories.Dapper.Mapping;

namespace Modern.Repositories.Dapper.Providers;

/// <summary>
/// The SQL query provider definition
/// </summary>
public interface IQueryProvider
{
    /// <summary>
    /// Returns an insert SQL query for a single entity
    /// </summary>
    /// <param name="mapping">Dapper entity mapping</param>
    string GetInsertWithOutputCommand<TEntity>(DapperEntityMapping<TEntity> mapping)
        where TEntity : class;
}