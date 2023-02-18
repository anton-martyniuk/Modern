using Bogus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modern.Controllers.CQRS.DataStore.Examples.Customized.CQRS;
using Modern.Controllers.CQRS.DataStore.Examples.Customized.DbContexts;
using Modern.Controllers.CQRS.DataStore.Examples.Customized.Entities;
using Modern.Controllers.CQRS.DataStore.Examples.Customized.Models;
using Modern.Controllers.CQRS.DataStore.Examples.Customized.Repositories;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.Repositories.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(x => x.ValidateScopes = false);

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
        options.AddRepository<CityDbContext, CityDbo, int>();
        options.AddConcreteRepository<ICityRepository, CityRepository>();
    })
    .AddCqrs(options =>
    {
        options.AddQueriesCommandsAndHandlersFor<CityDto, CityDbo, int, IModernRepository<CityDbo, int>>();
    })
    .AddCqrsControllers(options =>
    {
        options.AddController<CreateCityRequest, UpdateCityRequest, CityDto, CityDbo, int>();
    });

// Register custom handler
builder.Services.AddTransient<IRequestHandler<GetCountryCitiesQuery, List<CityDto>>, GetCountryCitiesQueryHandler>();


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

var mediator = app.Services.GetRequiredService<IMediator>();

var countQuery = new GetCountAllQuery<CityDto, int>();
var count = await mediator.Send(countQuery);
if (count == 0)
{
    // Create entities
    var newCities = GetCities(100);

    var createCommand = new CreateEntitiesCommand<CityDto>(newCities);
    await mediator.Send(createCommand);
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