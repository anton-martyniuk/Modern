## Services
Modern generic services use Modern generic repositories to perform CRUD operations.
To use **Service** install the `Modern.Services.DataStore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddServices(options =>
    {
        options.AddService<AirplaneDto, AirplaneDbo, long, IModernRepository<AirplaneDbo, long>>();
    });
```
Specify the type of Dto and dbo entity models, primary key and modern repository.\
Service requires one of modern repositories to be registered.