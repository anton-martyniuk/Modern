using Bogus;
using Microsoft.EntityFrameworkCore;
using Modern.Controllers.DataStore.Cached.Examples.DbContexts;
using Modern.Controllers.DataStore.Cached.Examples.Entities;
using Modern.Controllers.DataStore.Cached.Examples.Models;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.Abstractions;

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

// Add modern stuff
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<CityDbContext, CityDbo, int>(true);
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
        options.AddService<CityDto, CityDbo, int, IModernRepository<CityDbo, int>>();
    })
    .AddControllers(options =>
    {
        options.AddController<CreateCityRequest, UpdateCityRequest, CityDto, CityDbo, int>();
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

var service = app.Services.GetRequiredService<IModernService<CityDto, CityDbo, int>>();

var count = await service.CountAsync();
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);
    await service.CreateAsync(newCities);
}

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