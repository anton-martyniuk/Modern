using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.Abstractions;
using Modern.Services.DataStore.Cached.Examples.DbContexts;
using Modern.Services.DataStore.Cached.Examples.Entities;
using Modern.Services.DataStore.Cached.Examples.Models;
using Modern.Services.DataStore.Cached.Examples.Repositories;
using Modern.Services.DataStore.Cached.Examples.Services;

// ReSharper disable PossibleMultipleEnumeration

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;
var redisConnectionString = config.GetSection("Redis:ConnectionString").Value;

var services = new ServiceCollection();

// Needed for Services
services.AddLogging(options => options.SetMinimumLevel(LogLevel.Trace));

// Register db dependencies
services.AddDbContextFactory<CityDbContext>(x => x.EnableSensitiveDataLogging().UseSqlite(connectionString));

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
    })
    .AddRedisCache(options =>
    {
        options.AddCache<CityDto, int>();
        
        options.RedisConfiguration.ConnectionString = redisConnectionString;
        options.RedisConfiguration.AbortOnConnectFail = false;
        options.RedisCacheSettings.ExpiresIn = TimeSpan.FromSeconds(60);
    })
    .AddCachedServices(options =>
    {
        // Add service by entity type.
        // Use it when an own service is NOT needed
        options.AddService<CityDto, CityDbo, int, IModernRepository<CityDbo, int>>();
        
        // Or specify a concrete repository if available
        //options.AddService<CityDto, CityDbo, int, ICityRepository>();
        
        // Add concrete service inherited from IModernService
        // Use it when an own service with specific methods is needed
        options.AddConcreteService<ICityService, CityCachedService>();
    });

var provider = services.BuildServiceProvider();

// Use EF Core migrations to create a database
using (var scope = provider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
}

// Get service by entity type
var serviceByEntityType = provider.GetRequiredService<IModernService<CityDto, CityDbo, int>>();
var entities = await serviceByEntityType.GetAllAsync();

// Get concrete service
var service = provider.GetRequiredService<ICityService>();

var count = await service.CountAsync();
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);
    await service.CreateAsync(newCities);
    
    // All entities are cached
}

// Get entities
Console.WriteLine("All entities:");
var allEntities = await service.GetAllAsync();
Print(allEntities);

// Returns cached entity
var firstEntity = await service.GetByIdAsync(5);
firstEntity.Population += 10_000;

// Update entity
await service.UpdateAsync(firstEntity.Id, firstEntity);

// Get filtered entities
Console.WriteLine("\nCities in USA:");
var filteredEntities = await service.GetCountryCitiesAsync("USA");
Print(filteredEntities);

// Delete entity
await service.DeleteAsync(firstEntity.Id);

// Get entities
Console.WriteLine("\nAll entities:");
allEntities = await service.GetAllAsync();
Print(allEntities);

// ================================================================ END

void Print(IEnumerable<CityDto> entities)
{
    foreach (var entity in entities)
    {
        Console.WriteLine(entity.ToString());
    }
}

List<CityDto> GetCities(int count)
{
    return new Faker<CityDto>()
        .Ignore(x => x.Id)
        .RuleFor(x => x.Name, f => f.Address.City())
        .RuleFor(x => x.Country, f => f.Address.Country())
        .RuleFor(x => x.Area, f => f.Address.Random.Double() * 100_000)
        .RuleFor(x => x.Population, f => f.Random.Int(10_000, 1_000_000))
        .Generate(count);
}