using System.Text;
using Modern.Repositories.Dapper.Mapping;

namespace Modern.Repositories.Dapper.Adapters;

/// <summary>
/// The <see cref="ISqlQueryProvider"/> implementation for Postgres database
/// </summary>
public class PostgresSqlQueryProvider : ISqlQueryProvider
{
    /// <summary>
    /// <inheritdoc cref="ISqlQueryProvider.GetInsertWithOutputCommand{TEntity}"/>
    /// </summary>
    public string GetInsertWithOutputCommand<TEntity>(DapperEntityMapping<TEntity> entityMapping)
        where TEntity : class
    {
        var columns = entityMapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\"",
            builder.Length > 0 ? ", " : "", mapping.Key), builder => builder.ToString());

        var values = entityMapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}@{1}",
            builder.Length > 0 ? ", " : "", mapping.Value), builder => builder.ToString());

        //var outputString = Mapping.ColumnMappingsWithId.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}INSERTED.{1}",
        //    builder.Length > 0 ? ", " : "", mapping.Key), builder => builder.ToString());

        var outputString = entityMapping.ColumnMappingsWithId.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\" as {2}",
            builder.Length > 0 ? ", " : "", mapping.Key, mapping.Value), builder => builder.ToString());

        var queryBuilder = new StringBuilder();
        queryBuilder.AppendFormat("insert into {0} ({1}) values ({2})", entityMapping.TableName, columns, values);
        queryBuilder.AppendFormat(" RETURNING {0}", outputString);

        return queryBuilder.ToString();
    }
}