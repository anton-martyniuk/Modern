## Controllers In Memory
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