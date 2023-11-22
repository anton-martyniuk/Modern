## Modern MongoDB Repository
To use **MongoDB** repository install the `Modern.Repositories.MongoDB.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesMongoDb(options =>
    {
        options.ConfigureMongoDbClient(mongoDbConnectionString);
        options.AddRepository<AirplaneDbo, string>("database_name", "collection_name");
    });
```
Specify the type of Dbo entity model and "_id" key.\
Provide the connection string in `ConfigureMongoDbClient` method.
You can also use the second parameter `updateSettings` and configure the custom parameters in a `MongoClientSettings` class of MongoDB Driver.