namespace Modern.Repositories.Dapper.Mapping;

/// <summary>
/// The abstract <see cref="IDapperEntityMapping"/> implementation
/// </summary>
public abstract class DapperEntityMapping : IDapperEntityMapping
{
    /// <summary>
    /// Name of the database table
    /// </summary>
    public string TableName { get; private set; } = default!;

    /// <summary>
    /// Name of the Id column name in the database table
    /// </summary>
    public string IdColumnName { get; private set; } = default!;

    /// <summary>
    /// Collection of mapping of database column and entity property.<br/>
    /// The key should be a entity property name. The value should be a database column name
    /// </summary>
    public Dictionary<string, string> ColumnMappings { get; private set; } = default!;

    /// <summary>
    /// Sets a name of the database table
    /// </summary>
    /// <param name="tableName">Name of the database table</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping Table(string tableName)
    {
        TableName = tableName;
        return this;
    }

    /// <summary>
    /// Sets a name of the Id column name in the database table
    /// </summary>
    /// <param name="idColumnName">Name of id column</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping Id(string idColumnName)
    {
        IdColumnName = idColumnName;
        return this;
    }

    /// <summary>
    /// Sets a column mapping in the database table
    /// </summary>
    /// <param name="propertyName">Name of entity property</param>
    /// <param name="columnName">Name of the column of the corresponding database table</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping Column(string propertyName, string columnName)
    {
        ColumnMappings[propertyName] = columnName;
        return this;
    }
}
