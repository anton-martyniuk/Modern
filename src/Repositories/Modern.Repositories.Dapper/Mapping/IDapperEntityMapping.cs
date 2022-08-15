namespace Modern.Repositories.Dapper.Mapping;

/// <summary>
/// The Dapper entity mapping definition
/// </summary>
public interface IDapperEntityMapping
{
    /// <summary>
    /// Name of the database table
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Name of the Id column name in the database table
    /// </summary>
    public string IdColumnName { get; }

    /// <summary>
    /// Collection of mapping of database column and entity property.<br/>
    /// The key should be a entity property name. The value should be a database column name
    /// </summary>
    public Dictionary<string, string> ColumnMappings { get; }
}
