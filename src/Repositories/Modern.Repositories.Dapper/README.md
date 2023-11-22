## Modern Dapper Repository
To use **Dapper** repository install the `Modern.Repositories.Dapper.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesDapper(options =>
    {
        options.ProvideDatabaseConnection(() => new NpgsqlConnection(connectionString));
        options.AddRepository<AirplaneDapperMapping, AirplaneDbo, long>();
    });
```
Specify the type of Dapper mapping class, Dbo entity model and primary key.\
A dapper needs to know how to create a database connection.
Since there are multiple database connection classes -  provide the needed one using `ProvideDatabaseConnection` method.
Dapper repository requires to have a small mapping class that way generic repository can match the entity property name with database table column.
> Mapping class for Dapper is a part of Modern tools and not a part of Dapper library

For example consider the following mapping class:
```csharp
public class AirplaneDapperMapping : DapperEntityMapping<AirplaneDbo>
{
    protected override void CreateMapping()
    {
        Table("db_schema_name.airplanes")
            .Id(nameof(AirplaneDbo.Id), "id")
            .Column(nameof(AirplaneDbo.YearOfManufacture), "year_of_manufacture")
            .Column(nameof(AirplaneDbo.ModelId), "model_id")
            //...
            ;
    }
}
```