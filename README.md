# Modern

## What is Modern?
Modern is a set of modern .NET tools :hammer: :wrench: for fast and efficient development of common backend tasks.
It allows to create a product ready applications with just a configuration and set of model classes.
Modern tool are flexible, easily changeable and extendable.\
Modern includes the following components:
* generic repositories for SQL and NoSQL databases
* generic services with and without caching support
* generic in memory services with filtering in memory capabilities
* in memory and redis generic caches
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
As a result a production ready API will be created:

**GET** :large_blue_circle: \
`/Airplanes/get/{id}`

**GET** :large_blue_circle: :large_blue_circle: \
`/Airplanes/get`

**POST** :white_check_mark: \
`/Airplanes/create`

**POST** :white_check_mark: :white_check_mark: \
`/Airplanes/create-many`

**PUT** :part_alternation_mark: \
`/Airplanes/update/{id}`

**PUT** :part_alternation_mark: :part_alternation_mark: \
`/Airplanes/update-many`

**PATCH** :heavy_dollar_sign: \
`/Airplanes/patch/{id}`

**DELETE** :x: \
`/Airplanes/delete/{id}`

**DELETE** :x: :x: \
`/Airplanes/delete-many`

## List of Modern components :bookmark_tabs:

* [Repositories for SQL databases](#repositories-for-sql-databases)
* [Repositories for NoSQL databases](#repositories-for-no-sql-databases)
* [Services](#services)
* [Services with caching](#services-with-caching)
* [Services In Memory](#services-in-memory)
* [CQRS](#cqrs)
* [CQRS with caching](#cqrs-with-caching)
* [CQRS In Memory](#cqrs-in-memory)
* [GraphQL](#graphql)
* [GraphQL In Memory](#graphql-in-memory)
* [Controllers](#controllers)
* [Controllers In Memory](#controllers-in-memory)
* [OData Controllers](#odata-controllers)
* [OData Controllers In Memory](#odata-controllers-in-memory)

## Roadmap :arrow_right: :date:
The following features will be implemented in the next releases:
* Github Wiki docs
* Assembly scan in DI packages
* Unit and integration tests
* CQRS InMemory
* GraphQL tools
* MinimalsApis using FastEndpoints
* Reflection improvements
* Upgrade to .NET 7 (after official release in November)

## Repositories for SQL databases :pencil:
Modern generic repositories are built on top of 2 most popular ORM frameworks: **EF Core** and **Dapper**.
To use **EF Core** repository install the `Modern.Repositories.EFCore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<FlyingDbContext, AirplaneDbo, long>(useDbFactory: false);
    });
```
Specify the type of EF Core DbContext, Dbo entity model and primary key.
`useDbFactory` parameter indicates whether repository with DbContextFactory should be used. The default value is `false`.
> :information_source: Use this parameter if you plan to inherit from this generic repository and extend or change its functionality.\
When using `DbContextFactory` every repository creates and closes a database connection in each method.\
When NOT using `DbContextFactory` repository shares the same database connection during its lifetime.

> :warning: It is not recommended to use `useDbFactory = false` when repository is registered as SingleInstance,
> otherwise a single database connection will persists during the whole application lifetime

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
> :information_source: Mapping class for Dapper is a part of Modern tools and not a part of Dapper library

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

## Repositories for No SQL databases :pencil:
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

## Services :pencil:
Modern generic services use Modern generic repositories to perform CRUD operations.
To use **Service** install the `Modern.Services.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddServices(options =>
    {
        options.AddService<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Specify the type of Dto, dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.

## Services with caching :pencil:
Modern generic services with caching support use Modern generic repositories and cache to perform CRUD operations.
To use **Service with caching** install the `Modern.Services.DataStore.Cached.DependencyInjection` and `Modern.Cache.InMemory.DependencyInjection` Nuget packages and register them within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryCache(options =>
    {
        options.AddCache<AirplaneDto, long>();
        
        options.CacheSettings.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        options.CacheSettings.SlidingExpiration = TimeSpan.FromMinutes(10);
    })
    .AddCachedServices(options =>
    {
        options.AddService<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Or install the `Modern.Services.DataStore.Cached.DependencyInjection` and `Modern.Cache.Redis.DependencyInjection` Nuget packages and register them within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRedisCache(options =>
    {
        options.AddCache<AirplaneDto, long>();

        options.RedisConfiguration.ConnectionString = redisConnectionString;
        options.RedisConfiguration.AbortOnConnectFail = false;
        options.RedisCacheSettings.ExpiresIn = TimeSpan.FromMinutes(30);
    })
    .AddCachedServices(options =>
    {
        options.AddService<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```

When registering service specify the type of Dto, dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.\
When using **InMemoryCache** modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.\
When using **RedisCache** modify the `RedisConfiguration` of `StackExchange.Redis` package and `RedisCacheSettings` expiration time.

## Services In Memory :pencil:
Modern generic in memory services use Modern generic repositories and in memory cache to perform CRUD operations.
In Memory Services holds all the data in cache and performs filtering in the memory. While CachedService only use cache for the items it retrieves frequently.
To use **In Memory Service** install the `Modern.Services.DataStore.InMemory.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryServices(options =>
    {
        options.AddService<AirplaneDbo, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Specify the type of Dto, dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.\
The cache is registered under the hood and there is no Redis support as Redis doesn't support LINQ expressions on all the items.
The cache can be changed to the custom one if needed.

## CQRS :pencil:
Modern CQRS tools consist of Queries and Commands which use Modern generic repositories to perform CRUD operations.
To use **CQRS** install the `Modern.CQRS.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddCqrs(options =>
    {
        options.AddQueriesCommandsAndHandlersFor<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Specify the type of Dto, dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.

## CQRS with caching :pencil:
Modern generic services with caching support use Modern generic repositories and cache to perform CRUD operations.
To use **Service with caching** install the `Modern.Services.DataStore.Cached.DependencyInjection` and `Modern.Cache.InMemory.DependencyInjection` Nuget packages and register them within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryCache(options =>
    {
        options.AddCache<AirplaneDto, long>();
        
        options.CacheSettings.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        options.CacheSettings.SlidingExpiration = TimeSpan.FromMinutes(10);
    })
    .AddCachedCqrs(options =>
    {
        options.AddQueriesCommandsAndHandlersFor<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Or install the `Modern.Services.DataStore.Cached.DependencyInjection` and `Modern.Cache.Redis.DependencyInjection` Nuget packages and register them within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRedisCache(options =>
    {
        options.AddCache<AirplaneDto, long>();

        options.RedisConfiguration.ConnectionString = redisConnectionString;
        options.RedisConfiguration.AbortOnConnectFail = false;
        options.RedisCacheSettings.ExpiresIn = TimeSpan.FromMinutes(30);
    })
    .AddCachedCqrs(options =>
    {
        options.AddQueriesCommandsAndHandlersFor<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```

Specify the type of Dto, dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.\
When using **InMemoryCache** modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.\
When using **RedisCache** modify the `RedisConfiguration` of `StackExchange.Redis` package and `RedisCacheSettings` expiration time.

## Controllers :pencil:
Modern generic controllers use Modern generic services to perform CRUD operations.
To use **Controller** install the `Modern.Controllers.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddControllers(options =>
    {
        options.AddController<CreateAirplaneRequest, UpdateAirplaneRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires one of modern services to be registered: regular one or with caching.

## Controllers In Memory :pencil:
Modern generic controllers use Modern generic services to perform CRUD operations.
To use **In Memory Controller** install the `Modern.Controllers.DataStore.InMemory.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryControllers(options =>
    {
        options.AddController<CreateAirplaneRequest, UpdateAirplaneRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires a modern in memory service to be registered.

## OData Controllers :pencil:
Modern generic OData controllers use Modern generic repositories to perform OData queries.
To use **OData Controller** install the `Modern.Controllers.DataStore.OData.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddODataControllers(options =>
    {
        options.AddController<AirplaneDbo, long>();
    });
```
Specify the type of dto entity model and primary key.\
OData Controller requires one of modern repositories to be registered.

Also register OData in the DI:
```csharp
builder.Services.AddControllers(options =>
{
})
//..
.AddOData(opt =>
{
    // Adjust settings as appropriate
    opt.AddRouteComponents("api/odata", GetEdmModel());
    opt.Select().Filter().Count().SkipToken().OrderBy().Expand().SetMaxTop(1000);
    opt.TimeZone = TimeZoneInfo.Utc;
});

IEdmModel GetEdmModel()
{
    // Adjust settings as appropriate
    var builder = new ODataConventionModelBuilder();
    builder.EnableLowerCamelCase();

    // Register your OData models here. Name of the EntitySet should correspond to the name of OData controller
    builder.EntitySet<AirplaneDbo>("airplanes");
    builder.EntityType<AirplaneDbo>();

    return builder.GetEdmModel();
}
```

## OData Controllers In Memory :pencil:
Modern generic OData controllers use Modern generic repositories to perform OData queries.
To use **In Memory OData Controller** install the `Modern.Controllers.DataStore.InMemory.OData.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryODataControllers(options =>
    {
        options.AddController<AirplaneDbo, long>();
    });
```
Specify the type of dto entity model and primary key.\
OData Controller requires one of modern repositories to be registered.\
Remember to configure OData in DI as mentioned in see [OData Controllers](#odata-controllers)
