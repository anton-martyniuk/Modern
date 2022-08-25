# Modern

## What is Modern?
Modern is a set of modern .NET tools for fast and efficient development of common backend tasks.
It includes the following components:
* generic repositories for SQL and NoSQL databases
* generic services with and without caching support
* generic in memory services with filtering in memory capabilities
* generic set of CQRS queries and commands over repository (if you prefer CQRS over services)
* GraphQL queries and subscriptions
* generic controllers for all types of services
* OData controllers for all types of services

See docs here !!!

## How to get started?
First, select and install the needed Nuget packages !!!\
Lets consider the following example: repository, service and controller.
Install the following Nuget packages:
* Modern.Repositories.EFCore.DependencyInjection
* Modern.Services.DataStore.DependencyInjection
* Modern.Controllers.DataStore.DependencyInjection

Register the Modern builder in DI and add the following components:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<FlyingDbContext, AirplaneDbo, long>();
    })
    .AddServices(options =>
    {
        options.AddService<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    })
    .AddControllers(options =>
    {
        options.AddController<CreateAirplaneRequest, UpdateAirplaneRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Any of these components can be used separately. 

## List of Modern components

* [Repositories for SQL databases](#repositories-for-sql-databases)
* [Repositories for NoSQL databases](#repositories-for-no-sql-databases)
* [Service](#service)
* [Service with caching support](#service-with-caching)
* [In Memory Service](#in-memory-service)
* [CQRS](#cqrs)
* [CQRS with caching support](#cqrs-with-caching)
* [CQRS In Memory](#in-memory-cqrs)
* [GraphQL](#graphql)
* [GraphQL In Memory](#in-memory-graphql)
* [Controllers](#controllers)
* [Controllers In Memory](#in-memory-controllers)
* [OData Controllers](#odata-controllers)
* [OData Controllers In Memory](#in-memory-odata-controllers)

## Repositories for SQL databases
Modern generic repositories are built on top of 2 most popular ORM frameworks: **EF Core** and **Dapper**.
To use **EF Core** repository install the `Modern.Repositories.EFCore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<FlyingDbContext, AirplaneDbo, long>(useDbFactory: false, lifetime: ServiceLifetime.Transient);
    });
```
Specify the type of EF Core DbContext, Dbo entity model and primary key.
`useDbFactory` parameter indicates whether repository with DbContextFactory should be used. The default value is `false`.\
> Use this parameter if you plan to inherit from this generic repository and extend or change its functionality.\
When using `DbContextFactory` every repository creates and closes a database connection in each method.\
When NOT using `DbContextFactory` repository shares the same database connection during its lifetime.

Set the desired lifetime of the repository as needed or use `ServiceLifetime.Transient` by default.

To use **Dapper** repository install the `Modern.Repositories.Dapper.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesDapper(options =>
    {
        options.ProvideDatabaseConnection(() => new NpgsqlConnection(connectionString));
        options.AddRepository<AirplaneDapperMapping, AirplaneDbo, long>(lifetime: ServiceLifetime.Transient);
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

## Repositories for No SQL databases
Modern generic repositories are built on top one of the most popular NoSQL databases: **MongoDB**.
To use **MongoDB** repository install the `Modern.Repositories.MongoDB.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesMongoDb(options =>
    {
        options.ConfigureMongoDbClient(mongoDbConnectionString);
        options.AddRepository<AirplaneDbo, string>("database_name", "collection_name");
    });
```
Specify the type of Dbo entity model and "_id" key.\
Provide the connection string in `ConfigureMongoDbClient` method.
You can also use the second parameter `updateSettings` and configure the custom parameters in a `MongoClientSettings` class of MongoDB Driver.