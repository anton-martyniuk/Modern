## Modern LiteDB Repository
To use **LiteDB Async** repository install the `Modern.Repositories.LiteDB.Async.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesLiteDbAsync(options =>
    {
        options.AddRepository<AirplaneDbo, string>("connection_string", "collection_name");
    });
```
Specify the type of Dbo entity model and "_id" key.