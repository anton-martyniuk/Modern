using System.Text.Json;
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.Dapper.Examples.Entities;
using Modern.Dapper.Examples.Mapping;
using Modern.Dapper.Examples.Repositories;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;

// ReSharper disable PossibleMultipleEnumeration

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;

// Use fluent migrator library to create a database
using (var serviceProvider = CreateFluentMigratorServices(connectionString!))
using (var scope = serviceProvider.CreateScope())
{
    // Put the database update into a scope to ensure
    // that all resources will be disposed.
    UpdateDatabase(scope.ServiceProvider);
}

var services = new ServiceCollection();

// Add modern stuff
services
    .AddModern()
    .AddRepositoriesDapper(options =>
    {
        options.ProvideDatabaseConnection(() => new SqliteConnection(connectionString));
        
        // Add repository by entity type.
        // Use it when an own repository is NOT needed
        options.AddRepository<CityDboMapping, CityDbo, int>();
        
        // Add concrete repository inherited from IModernRepository
        // Use it when an own repository with specific methods is needed
        options.AddConcreteRepository<ICityRepository, CityRepository, CityDboMapping>();
    });

var provider = services.BuildServiceProvider();

// Get repository by entity type
var repositoryByEntityType = provider.GetRequiredService<IModernRepository<CityDbo, int>>();
var entities = await repositoryByEntityType.GetAllAsync();

// Get concrete repository
var repository = provider.GetRequiredService<ICityRepository>();

var count = await repository.CountAsync();
if (count == 0)
{
    var newEntities = new List<CityDbo>
    {
        new()
        {
            Name = "New York",
            Country = "USA",
            Area = 783.8,
            Population = 8_175_133
        },
        new()
        {
            Name = "Los Angeles",
            Country = "USA",
            Area = 1_301.97,
            Population = 3_884_307
        },
        new()
        {
            Name = "London",
            Country = "Great Britain",
            Area = 1_572.15,
            Population = 8_908_081
        },
        new()
        {
            Name = "Oslo",
            Country = "USA",
            Area = 454.82,
            Population = 702_543
        },
        new()
        {
            Name = "Kyiv",
            Country = "Ukraine",
            Area = 839.10,
            Population = 2_884_000
        }
    };

    // Create entities
    await repository.CreateAsync(newEntities);
}

// Get entities
Console.WriteLine("All entities:");
var allCities = await repository.GetAllAsync();
Print(allCities);

var firstEntity = allCities.First();
firstEntity.Population += 10_000;

// Update entity
await repository.UpdateAsync(firstEntity.Id, firstEntity);

// Get filtered entities
Console.WriteLine("\nCities in Ukraine:");
var ukrainianCities = await repository.GetCountryCitiesAsync("Ukraine");
Print(ukrainianCities);

// Delete entity
await repository.DeleteAsync(firstEntity.Id);

// Get entities
Console.WriteLine("\nAll entities:");
allCities = await repository.GetAllAsync();
Print(allCities);

void Print(IEnumerable<CityDbo> entities)
{
    foreach (var entity in entities)
    {
        Console.WriteLine($"{JsonSerializer.Serialize(entity)}");
    }
}

static ServiceProvider CreateFluentMigratorServices(string connectionString)
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(builder => builder
            .AddSQLite()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(ICityRepository).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

static void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}