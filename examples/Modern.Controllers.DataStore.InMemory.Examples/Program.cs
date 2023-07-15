using Bogus;
using Microsoft.EntityFrameworkCore;
using Modern.Controllers.DataStore.Examples.DbContexts;
using Modern.Controllers.DataStore.Examples.Entities;
using Modern.Controllers.DataStore.Examples.Models;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Services.DataStore.InMemory.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("DatabaseConfiguration:ConnectionString").Value;

// Needed for Services
builder.Services.AddLogging();

// Register db dependencies
builder.Services.AddDbContextFactory<CityDbContext>(x => x.EnableSensitiveDataLogging().UseSqlite(connectionString));

// Add modern stuff
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepositoryWithDbFactory<CityDbContext, CityDbo, int>();
    })
    .AddInMemoryServices(options =>
    {
        options.AddService<CityDto, CityDbo, int>();
    })
    .AddInMemoryControllers(options =>
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
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();
await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

var service = scope.ServiceProvider.GetRequiredService<IModernInMemoryService<CityDto, CityDbo, int>>();

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