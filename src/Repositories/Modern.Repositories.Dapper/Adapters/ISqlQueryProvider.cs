using Modern.Repositories.Dapper.Mapping;

namespace Modern.Repositories.Dapper.Adapters;

/// <summary>
/// The SQL query provider definition
/// </summary>
internal interface ISqlQueryProvider
{
    /// <summary>
    /// Returns an insert SQL query for a single entity
    /// </summary>
    /// <param name="mapping">Dapper entity mapping</param>
    string GetInsertWithOutputCommand<TEntity>(DapperEntityMapping<TEntity> mapping)
        where TEntity : class;
}