## Controllers CQRS
Modern generic CQRS controllers use Modern CQRS Commands and Queries to perform CRUD operations.
To use **CQRS Controller** install the `Modern.Controllers.CQRS.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddCqrsControllers(options =>
    {
        options.AddController<CreateAirplaneRequest, UpdateAirplaneRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires CQRS Commands and Queries to be registered: regular one or with caching.