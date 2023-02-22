using Bogus;
using Microsoft.EntityFrameworkCore;
using Modern.EfCore.UnitOfWork.Examples.Controllers;
using Modern.EfCore.UnitOfWork.Examples.DbContexts;
using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.EfCore.UnitOfWork.Examples.Models;
using Modern.EfCore.UnitOfWork.Examples.Repositories;
using Modern.EfCore.UnitOfWork.Examples.Services;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.EFCore.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("DatabaseConfiguration:ConnectionString").Value;

// Needed for Services
builder.Services.AddLogging();

// Register db dependencies
builder.Services.AddDbContext<CityDbContext>(x => x.EnableSensitiveDataLogging().UseSqlite(connectionString));

// Add modern stuff
builder.Services
    .AddModern()
    .AddRepositoriesEfCore(options =>
    {
        options.AddRepositoryWithUnitOfWork<CityDbContext, CityDbo, int>(ServiceLifetime.Scoped);
        options.AddConcreteRepository<ICityRepository, CityRepository>(ServiceLifetime.Scoped);
    })
    .AddServices(options =>
    {
        options.AddConcreteService<ICityService, CityService>(ServiceLifetime.Scoped);
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
    
    // Get concrete service
    var service = scope.ServiceProvider.GetRequiredService<ICityService>();

    var count = await service.CountAsync();
    if (count == 0)
    {
        // Create entities
        var newCities = GetCities(100);
        await service.CreateAsync(newCities);
    }
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