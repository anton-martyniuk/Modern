using System.Text.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.EfCore.Examples.DbContexts;
using Modern.EfCore.Examples.Entities;
using Modern.EfCore.Examples.Repositories;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;

// ReSharper disable PossibleMultipleEnumeration

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;

var services = new ServiceCollection();

// Register db dependencies
services.AddDbContextFactory<CityDbContext>(x => x.EnableSensitiveDataLogging().UseSqlite(connectionString));

// Add modern stuff
services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        // Add repository by entity type.
        // Use it when an own repository is NOT needed
        options.AddRepository<CityDbContext, CityDbo, int>();
        
        // Add concrete repository inherited from IModernRepository
        // Use it when an own repository with specific methods is needed
        options.AddConcreteRepository<ICityRepository, CityRepository>();
    });

var provider = services.BuildServiceProvider();

// Use EF Core migrations to create a database
using (var scope = provider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
}

// Get repository by entity type
var repositoryByEntityType = provider.GetRequiredService<IModernRepository<CityDbo, int>>();
var entities = await repositoryByEntityType.GetAllAsync();

// Get concrete repository
var repository = provider.GetRequiredService<ICityRepository>();

var count = await repository.CountAsync();
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);
    await repository.CreateAsync(newCities);
}

// Get entities
Console.WriteLine("All entities:");
var allEntities = await repository.GetAllAsync();
Print(allEntities);

var firstEntity = allEntities.First();
firstEntity.Population += 10_000;

// Update entity
await repository.UpdateAsync(firstEntity.Id, firstEntity);

// Get filtered entities
Console.WriteLine("\nCities in USA:");
var filteredEntities = await repository.GetCountryCitiesAsync("USA");
Print(filteredEntities);

// Delete entity
await repository.DeleteAsync(firstEntity.Id);

// Get entities
Console.WriteLine("\nAll entities:");
allEntities = await repository.GetAllAsync();
Print(allEntities);

// ================================================================ END

void Print(IEnumerable<CityDbo> entities)
{
    foreach (var entity in entities)
    {
        Console.WriteLine($"{JsonSerializer.Serialize(entity)}");
    }
}

List<CityDbo> GetCities(int count)
{
    return new Faker<CityDbo>()
        .Ignore(x => x.Id)
        .RuleFor(x => x.Name, f => f.Address.City())
        .RuleFor(x => x.Country, f => f.Address.Country())
        .RuleFor(x => x.Area, f => f.Address.Random.Double() * 100_000)
        .RuleFor(x => x.Population, f => f.Random.Int(10_000, 1_000_000))
        .Generate(count);
}