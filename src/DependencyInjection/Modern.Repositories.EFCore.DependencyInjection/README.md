## Modern EF Core Repository
To use **EF Core** repository install the `Modern.Repositories.EFCore.DependencyInjection` Nuget package and register it within Modern builder in DI:
```csharp
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<FlyingDbContext, AirplaneDbo, long>(useDbFactory: false);
    });
```
Specify the type of EF Core DbContext, Dbo entity model and primary key.
`useDbFactory` parameter indicates whether repository with DbContextFactory should be used. The default value is `false`.
> Use this parameter if you plan to inherit from this generic repository and extend or change its functionality.\
When using `DbContextFactory` every repository creates and closes a database connection in each method.\
When NOT using `DbContextFactory` repository shares the same database connection during its lifetime.

> It is not recommended to use `useDbFactory = false` when repository is registered as SingleInstance,
> otherwise a single database connection will persist during the whole application lifetime