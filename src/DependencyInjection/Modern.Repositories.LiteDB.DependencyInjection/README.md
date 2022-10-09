## Modern LiteDB Repository
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