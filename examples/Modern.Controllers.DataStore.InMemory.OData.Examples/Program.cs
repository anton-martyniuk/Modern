using System.Text.Json.Serialization;
using Bogus;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Modern.Controllers.DataStore.InMemory.OData.Examples.DbContexts;
using Modern.Controllers.DataStore.InMemory.OData.Examples.Entities;
using Modern.Controllers.DataStore.InMemory.OData.Examples.Models;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;
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

// Add controllers and register OData
builder.Services.AddControllers(options =>
    {
        options.EnableEndpointRouting = true;
    })
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.PropertyNamingPolicy = null;
        x.JsonSerializerOptions.WriteIndented = true;
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddOData(opt =>
    {
        opt.AddRouteComponents("api/odata", GetEdmModel());
        opt.Select().Filter().Count().SkipToken().OrderBy().Expand().SetMaxTop(1000);
        opt.TimeZone = TimeZoneInfo.Utc;
    });

// Add modern stuff
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepository<CityDbContext, CityDbo, int>(true);
    })
    .AddInMemoryServices(options =>
    {
        options.AddService<CityDto, CityDbo, int, IModernRepository<CityDbo, int>>();
    })
    .AddInMemoryControllers(options =>
    {
        options.AddController<CreateCityRequest, UpdateCityRequest, CityDto, CityDbo, int>();
    })
    .AddInMemoryODataControllers(options =>
    {
        options.AddController<CityDto, int>();
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

var service = app.Services.GetRequiredService<IModernInMemoryService<CityDto, CityDbo, int>>();

var count = await service.CountAsync();
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);
    await service.CreateAsync(newCities);
}

// Run the api
app.Run();


static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EnableLowerCamelCase();

    builder.EntitySet<CityDto>("cities");
    builder.EntityType<CityDto>();

    return builder.GetEdmModel();
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