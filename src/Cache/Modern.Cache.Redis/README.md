## Modern Redis Cache
To use **Redis Cache** install the `Modern.Cache.Redis.DependencyInjection` Nuget packages and register cache within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRedisCache(options =>
    {
        options.AddCache<AirplaneDto, long>();

        options.RedisConfiguration.ConnectionString = redisConnectionString;
        options.RedisConfiguration.AbortOnConnectFail = false;
        options.RedisCacheSettings.ExpiresIn = TimeSpan.FromMinutes(30);
    });
```

Specify the Dto entity model and primary key types.
Modify the `RedisConfiguration` of `StackExchange.Redis` package and expiration time in `RedisCacheSettings`.
