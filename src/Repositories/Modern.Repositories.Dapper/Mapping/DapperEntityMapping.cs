namespace Modern.Repositories.Dapper.Mapping;

/// <summary>
/// The abstract dapper entity mapper class
/// </summary>
public abstract class DapperEntityMapping<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Name of the database table
    /// </summary>
    internal string TableName { get; private set; } = default!;

    /// <summary>
    /// Database id column and entity property mapping
    /// </summary>
    internal KeyValuePair<string, string> IdColumn { get; private set; }

    /// <summary>
    /// Collection of mapping of database column and entity property.<br/>
    /// The key should be a entity property name. The value should be a database column name
    /// </summary>
    internal Dictionary<string, string> ColumnMappings { get; } = new();

    /// <summary>
    /// Collection of mapping of database column and entity property with id column.<br/>
    /// The key should be a entity property name. The value should be a database column name
    /// </summary>
    internal Dictionary<string, string> ColumnMappingsWithId { get; } = new();

    /// <summary>
    /// Name of the Id column name in the database table
    /// </summary>
    internal string IdColumnName => IdColumn.Key;

    /// <summary>
    /// Creates mapping
    /// </summary>
    protected abstract void CreateMapping();

    /// <summary>
    /// Builds the mapping
    /// </summary>
    internal void Build()
    {
        // Build mapping only once
        if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(IdColumn.Key) && ColumnMappings.Count == 0)
        {
            return;
        }

        CreateMapping();

        foreach (var (key, value) in ColumnMappings)
        {
            ColumnMappingsWithId.TryAdd(key, value);
        }

        ColumnMappingsWithId.TryAdd(IdColumn.Key, IdColumn.Value);
    }

    /// <summary>
    /// Sets a name of the database table
    /// </summary>
    /// <param name="tableName">Name of the database table</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping<TEntity> Table(string tableName)
    {
        TableName = tableName;
        return this;
    }

    /// <summary>
    /// Sets a name of the Id column name in the database table
    /// </summary>
    /// <param name="propertyName">Name of entity id property</param>
    /// <param name="columnName">Name of the id column of the corresponding database table</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping<TEntity> Id(string propertyName, string columnName)
    {
        IdColumn = new KeyValuePair<string, string>(columnName, propertyName);
        return this;
    }

    /// <summary>
    /// Sets a column mapping in the database table
    /// </summary>
    /// <param name="propertyName">Name of entity property</param>
    /// <param name="columnName">Name of the column of the corresponding database table</param>
    /// <returns>DapperEntityMapping</returns>
    public DapperEntityMapping<TEntity> Column(string propertyName, string columnName)
    {
        ColumnMappings[columnName] = propertyName;
        return this;
    }
}
