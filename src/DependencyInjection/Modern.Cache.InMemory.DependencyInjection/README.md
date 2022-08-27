## Modern In Memory Cache
To use **In Memory Cache** install the `Modern.Cache.InMemory.DependencyInjection` Nuget packages and register cache within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryCache(options =>
    {
        options.AddCache<AirplaneDto, long>();
        
        options.CacheSettings.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        options.CacheSettings.SlidingExpiration = TimeSpan.FromMinutes(10);
    });
```
Specify the Dto entity model and primary key types.\
Modify the CacheSettings of type `MemoryCacheEntryOptions` to specify the cache expiration time.
