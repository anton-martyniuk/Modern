## Controllers
Modern generic controllers use Modern generic services to perform CRUD operations.
To use **Controller** install the `Modern.Controllers.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddControllers(options =>
    {
        options.AddController<CreateAirplaneRequest, UpdateAirplaneRequest, AirplaneDto, AirplaneDbo, long>();
    });
```
Specify the type of create and update requests, dto entity model and primary key.\
Controller requires one of modern services to be registered: regular one or with caching.