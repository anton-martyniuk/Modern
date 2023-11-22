## CQRS
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
Specify the type of Dto and dbo entity models, primary key and modern repository.\
CQRS requires one of modern repositories to be registered.