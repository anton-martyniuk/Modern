## Services In Memory
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