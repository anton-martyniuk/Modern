## OData Controllers In Memory
Modern generic OData controllers use Modern generic repositories to perform OData queries.
To use **In Memory OData Controller** install the `Modern.Controllers.DataStore.InMemory.OData.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddInMemoryODataControllers(options =>
    {
        options.AddController<AirplaneDbo, long>();
    });
```
Specify the type of dto entity model and primary key.\
OData Controller requires one of modern repositories to be registered.\

Also register OData in the DI:
```csharp
builder.Services.AddControllers(options =>
{
})
//..
.AddOData(opt =>
{
    // Adjust settings as appropriate
    opt.AddRouteComponents("api/odata", GetEdmModel());
    opt.Select().Filter().Count().SkipToken().OrderBy().Expand().SetMaxTop(1000);
    opt.TimeZone = TimeZoneInfo.Utc;
});

IEdmModel GetEdmModel()
{
    // Adjust settings as appropriate
    var builder = new ODataConventionModelBuilder();
    builder.EnableLowerCamelCase();

    // Register your OData models here. Name of the EntitySet should correspond to the name of OData controller
    builder.EntitySet<AirplaneDbo>("airplanes");
    builder.EntityType<AirplaneDbo>();

    return builder.GetEdmModel();
}
```