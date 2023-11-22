## CQRS with caching
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

Specify the type of Dto and dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.\
When using **InMemoryCache** modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.\
When using **RedisCache** modify the `RedisConfiguration` of `StackExchange.Redis` package and `RedisCacheSettings` expiration time.