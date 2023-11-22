# Modern

## What is Modern?

Modern is a set of modern .NET tools :hammer: :wrench: for fast and efficient development of common backend tasks.
It allows to create a production ready applications with just set of models and configuration which can be further extended.
Modern tool are flexible, easily changeable and extendable.\
It includes the following components:
* generic repositories for SQL and NoSQL databases
* generic services with and without caching support
* generic in memory services with in-memory filtering capabilities
* in-memory and redis generic caches
* generic set of CQRS queries and commands over repository (if you prefer CQRS over services)
* generic controllers for all types of services
* OData controllers for all types of services

For more information - [see full documentation here](https://github.com/anton-martyniuk/Modern/wiki).

Examples for all types of components - [see here](./examples).

---

## Table of contents :bookmark_tabs:

* [How to get started?](#how-to-get-started)
* [Roadmap](#roadmap)
* [Repositories](#repositories)
* [Repositories for SQL databases](#repositories-for-sql-databases)
* [Repositories for NoSQL databases](#repositories-for-no-sql-databases)
* [Services](#services)
* [Services with caching](#services-with-caching)
* [Services In Memory](#services-in-memory)
* [CQRS](#cqrs)
* [CQRS with caching](#cqrs-with-caching)
* [Controllers](#controllers)
* [Controllers CQRS](#controllers-cqrs)
* [Controllers In Memory](#controllers-in-memory)
* [OData Controllers](#odata-controllers)
* [OData Controllers In Memory](#odata-controllers-in-memory)

## How to get started?
Lets create a Web Api with CRUD operations over Airplane entities.
We need a repository, service and controller.
1. Install the following Nuget packages:
* [Modern.Repositories.EFCore.DependencyInjection](https://www.nuget.org/packages/Modern.Repositories.EFCore.DependencyInjection)
* [Modern.Services.DataStore.DependencyInjection](https://www.nuget.org/packages/Modern.Services.DataStore.DependencyInjection)
* [Modern.Controllers.DataStore.DependencyInjection](https://www.nuget.org/packages/Modern.Controllers.DataStore.DependencyInjection)

2. Create classes for the following models: AirplaneDto and AirplaneDbo.
3. Create an EF Core DbContext for accessing the AirplaneDbo.

4. Register the Modern builder in DI and add the following components:
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
        options.AddController<CreateRequest, UpdateRequest, AirplaneDto, AirplaneDbo, long>();
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

## Roadmap :arrow_right: :date:
The following features will be implemented in the next releases:
* Assembly scan in DI packages
* Unit and integration tests
* MinimalsApis
* Reflection improvements

## Repositories
Modern generic repository is divided into 2 interfaces: `IModernQueryRepository<TEntity, TId>` and `IModernCrudRepository<TEntity, TId>`.
`IModernQueryRepository` has the following methods:
```csharp
Task<TEntity> GetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<TEntity?> TryGetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<IEnumerable<TEntity>> GetAllAsync(EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<long> CountAsync(CancellationToken cancellationToken = default);

Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null,
    CancellationToken cancellationToken = default);

Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

Task<PagedResult<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default);

IQueryable<TEntity> AsQueryable();
```

`IModernCrudRepository` has the following methods:
```csharp
Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

Task<List<TEntity>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default);

Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

Task<TEntity> UpdateAsync(TId id, Action<TEntity> update, CancellationToken cancellationToken = default);

Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);

Task<bool> DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default);

Task<TEntity> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default);
```

## Repositories for SQL databases :pencil:
Modern generic repositories for SQL databases are built on top of 2 the following ORM frameworks:
* **EF Core**
* **Dapper**

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
> otherwise a single database connection will persist during the whole application lifetime

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
Modern generic repositories for No SQL databases are built on of the following NoSQL databases:
* **MongoDB**
* **LiteDb** (embedded single-file NoSQL database)

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

To use **LiteDB** repository install the `Modern.Repositories.LiteDB.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesLiteDb(options =>
    {
        options.AddRepository<AirplaneDbo, string>("connection_string", "collection_name");
    });
```
Specify the type of Dbo entity model and "_id" key.

> :information_source: Right out of the box LiteDB official library supports only synchronous methods. To use asynchronous methods a third party library can be used. Modern libraries support asynchronous methods in LiteDB using [litedb-async library](https://github.com/mlockett42/litedb-async)

> :warning: **DISCLAIMER:** LiteDb async repository uses litedb-async library which is not an official LiteDb project.
Modern libraries are NOT responsible for any problems with litedb-async library, so use this package at your own risk.

To use **LiteDB Async** repository install the `Modern.Repositories.LiteDB.Async.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesLiteDbAsync(options =>
    {
        options.AddRepository<AirplaneDbo, long>("connection_string", "collection_name");
    });
```
Specify the type of Dbo entity model and "_id" key.

## Services :pencil:
Modern generic service is divided into 2 interfaces: `IModernQueryService<TEntityDto, TEntityDbo, TId>` and `IModernCrudService<TEntityDto, TEntityDbo, TId>`.
`IModernQueryService` has the following methods:
```csharp
Task<TEntityDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

Task<TEntityDto?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default);

Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default);

Task<long> CountAsync(CancellationToken cancellationToken = default);

Task<long> CountAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default);

Task<bool> ExistsAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default);

Task<TEntityDto?> FirstOrDefaultAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default);

Task<TEntityDto?> SingleOrDefaultAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default);

Task<List<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, CancellationToken cancellationToken = default);

Task<PagedResult<TEntityDto>> WhereAsync(Expression<Func<TEntityDbo, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

IQueryable<TEntityDbo> AsQueryable();
```

`IModernCrudService` has the following methods:
```csharp
Task<TEntityDto> CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

Task<List<TEntityDto>> CreateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default);

Task<TEntityDto> UpdateAsync(TId id, TEntityDto entity, CancellationToken cancellationToken = default);

Task<List<TEntityDto>> UpdateAsync(List<TEntityDto> entities, CancellationToken cancellationToken = default);

Task<TEntityDto> UpdateAsync(TId id, Action<TEntityDto> update, CancellationToken cancellationToken = default);

Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);

Task<bool> DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default);

Task<TEntityDto> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default);
```


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
Specify the type of Dto and dbo entity models, primary key and modern repository.\
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

When registering service specify the type of Dto and dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.\
When using **InMemoryCache** modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.\
When using **RedisCache** modify the `RedisConfiguration` of `StackExchange.Redis` package and expiration time in `RedisCacheSettings`.

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
Specify the type of Dto and dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.\
The cache is registered under the hood and there is no Redis support as Redis doesn't support LINQ expressions on its items.
The cache can be changed to the custom one if needed.

## CQRS :pencil:
Modern generic CQRS consist of Commands and Queries.
CQRS has the following Queries:
```csharp
GetAllQuery<TEntityDto, TId>() : IRequest<List<TEntityDto>>

GetByIdQuery<TEntityDto, TId>(TId Id) : IRequest<TEntityDto>

TryGetByIdQuery<TEntityDto, TId>(TId Id) : IRequest<TEntityDto?>

GetCountAllQuery<TEntityDto, TId> : IRequest<long>

GetCountQuery<TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<long>

GetExistsQuery<TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<bool>

GetFirstOrDefaultQuery<TEntityDto, TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<TEntityDto?>

GetSingleOrDefaultQuery<TEntityDto, TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<TEntityDto?>

GetWhereQuery<TEntityDto, TEntityDbo, TId>(Expression<Func<TEntityDbo, bool>> Predicate) : IRequest<List<TEntityDto>>

GetWherePagedQuery<TEntityDto, TEntityDbo, TId> : IRequest<PagedResult<TEntityDto>>
```

CQRS has the following Commands:
```csharp
CreateEntityCommand<TEntityDto>(TEntityDto Entity) : IRequest<TEntityDto>

CreateEntitiesCommand<TEntityDto>(List<TEntityDto> Entities) : IRequest<List<TEntityDto>>

UpdateEntityCommand<TEntityDto, TId>(TId Id, TEntityDto Entity) : IRequest<TEntityDto>

UpdateEntityByActionCommand<TEntityDto, TId>(TId Id, Action<TEntityDto> UpdateAction) : IRequest<TEntityDto>

UpdateEntitiesCommand<TEntityDto>(List<TEntityDto> Entities) : IRequest<List<TEntityDto>>

DeleteEntityCommand<TId>(TId Id) : IRequest<bool>

DeleteEntitiesCommand<TId>(List<TId> Ids) : IRequest<bool>

DeleteAndReturnEntityCommand<TEntityDto, TId>(TId Id) : IRequest<TEntityDto>
```

Modern generic CQRS consist of Commands and Queries which use Modern generic repositories to perform CRUD operations.
To use **CQRS** install the `Modern.CQRS.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddCqrs(options =>
    {
        options.AddQueriesCommandsAndHandlersFor<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Specify the type of Dto and dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.

## CQRS with caching :pencil:
Modern generic CQRS Commands and Queries with caching support use Modern generic repositories and cache to perform CRUD operations.
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

Specify the type of Dto and dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.\
When using **InMemoryCache** modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.\
When using **RedisCache** modify the `RedisConfiguration` of `StackExchange.Redis` package and `RedisCacheSettings` expiration time.

## Controllers :pencil:
Modern generic controller has the following HTTP endpoints:
```csharp
[Route("api/[controller]")]
public class ModernController<TCreateRequest, TUpdateRequest, TEntityDto, TEntityDbo, TId> : ControllerBase
{
    [HttpGet("get/{id}")]
    Task<IActionResult> GetById([Required] TId id)
    
    [HttpGet("get")]
    Task<IActionResult> GetAll(CancellationToken cancellationToken)
    
    [HttpPost("create")]
    Task<IActionResult> Create([FromBody, Required] TCreateRequest request)
    
    [HttpPost("create-many")]
    Task<IActionResult> CreateMany([FromBody, Required] List<TCreateRequest> requests)
    
    [HttpPut("update/{id}")]
    Task<IActionResult> Update([Required] TId id, [FromBody, Required] TUpdateRequest request)
    
    [HttpPut("update-many")]
    Task<IActionResult> UpdateMany([FromBody, Required] List<TUpdateRequest> requests)
    
    [HttpPatch("patch/{id}")]
    Task<IActionResult> Patch([Required] TId id, [FromBody] JsonPatchDocument<TEntityDto> patch)
    
    [HttpDelete("delete/{id}")]
    Task<IActionResult> Delete([Required] TId id)
    
    [HttpDelete("delete-many")]
    Task<IActionResult> DeleteMany([Required] List<TId> ids)
}
```

Modern generic controllers use Modern generic services to perform CRUD operations.
To use **Controller** install the `Modern.Controllers.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddControllers(options =>
    {
        options.AddController<CreateRequest, UpdateRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires one of modern services to be registered: regular one or with caching.

## Controllers CQRS :pencil:
Modern generic CQRS controllers use Modern CQRS Commands and Queries to perform CRUD operations.
To use **CQRS Controller** install the `Modern.Controllers.CQRS.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddCqrsControllers(options =>
    {
        options.AddController<CreateRequest, UpdateRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires CQRS Commands and Queries to be registered: regular one or with caching.

## Controllers In Memory :pencil:
Modern generic controllers use Modern generic services to perform CRUD operations.
To use **In Memory Controller** install the `Modern.Controllers.DataStore.InMemory.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryControllers(options =>
    {
        options.AddController<CreateRequest, UpdateRequest, AirplaneDto, AirplaneDbo, long>();
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

## Support My Work :star2:

If you find this package helpful, consider supporting my work by buying me a coffee :coffee:!\
Your support is greatly appreciated and helps me continue developing and maintaining this project.\
You can also give me a :star: on github to make my package more relevant to others.

[![Buy me a coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/antonmartyniuk)

Thank you for your support!
