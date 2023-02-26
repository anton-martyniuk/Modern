using Bogus;
using Microsoft.EntityFrameworkCore;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Controllers;
using Modern.Controllers.DataStore.Cached.Examples.Customized.DbContexts;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Models;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Repositories;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Services;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Services.DataStore.Cached.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("DatabaseConfiguration:ConnectionString").Value;
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;

// Needed for Services
builder.Services.AddLogging();

// Register db dependencies
builder.Services.AddDbContextFactory<CityDbContext>(x => x.EnableSensitiveDataLogging().UseSqlite(connectionString));

// Register configuration
builder.Services
    .AddOptions<ModernCachedServiceConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(ModernCachedServiceConfiguration)));

// Add modern stuff
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
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
        // Specify configuration in appsettings.json and use AddOptions or configure service here:
        options.ConfigureService(configuration =>
        {
            configuration.AddToCacheWhenEntityCreated = true;
            configuration.AddOrUpdateInCacheWhenEntityIsUpdated = false;
        });
        
        options.AddConcreteService<ICityService, CityCachedService>();
    })
    .AddControllers(options =>
    {
        options.AddController<CitiesController>();
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Use EF Core migrations to create a database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
}

// Get concrete service
var service = app.Services.GetRequiredService<ICityService>();

var count = await service.CountAsync();
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);
    await service.CreateAsync(newCities);
}

// Run the api
app.Run();

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