using FluentMigrator;

namespace Modern.Dapper.Examples.Migrations;

[Migration(0001, "Initial migration", BreakingChange = false)]
public class InitialMigration : Migration
{
    private const string TableName = "cities";

    /// <summary>
    /// Performs database migration version UP
    /// </summary>
    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable().Unique()
            .WithColumn("country").AsString(100).NotNullable()
            .WithColumn("area").AsDecimal().NotNullable()
            .WithColumn("population").AsInt32().NotNullable();
    }

    /// <summary>
    /// Performs database migration version DOWN
    /// </summary>
    public override void Down()
    {
        Delete.Table(TableName);
    }
}