using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.LiteDb.Async.Examples.Entities;
using Modern.LiteDb.Async.Examples.Repositories;
using Modern.Repositories.Abstractions;

var services = new ServiceCollection();

services
    .AddModern()
    .AddRepositoriesLiteDbAsync(options =>
    {
        // Add repository by entity type.
        // Use it when an own repository is NOT needed
        options.AddRepository<CarDbo, long>("example_lite.db", "cars");
        
        // Add concrete repository inherited from IModernRepository
        // Use it when an own repository with specific methods is needed
        options.AddConcreteRepository<ICarRepository, CarRepository>();
    });

var provider = services.BuildServiceProvider();

// Get repository by entity type
var repositoryByEntityType = provider.GetRequiredService<IModernRepository<CarDbo, long>>();
var entities = await repositoryByEntityType.GetAllAsync();

// Get concrete repository
var repository = provider.GetRequiredService<ICarRepository>();

var mazda = new CarDbo
{
    Manufacturer = "Mazda",
    Model = "CX-5",
    Engine = "2.5 L Skyactiv-G Petrol",
    Drive = "AWD",
    YearOfProduction = 2023,
    Price = 40_00m
};

var mazda2 = new CarDbo
{
    Manufacturer = "Mazda",
    Model = "CX-60",
    Engine = "3.3 L Skyactiv-G Petrol",
    Drive = "AWD",
    YearOfProduction = 2023,
    Price = 50_00m
};

// Create entities
mazda = await repository.CreateAsync(mazda);
mazda2 = await repository.CreateAsync(mazda2);

// Get entities
var allCars = await repository.GetAllAsync();
var awdCars = await repository.FindCarsByDriveTypeAsync("AWD");

Console.WriteLine("All cars:");
PrintCars(allCars);

Console.WriteLine("\nAWD cars:");
PrintCars(awdCars);

// Update entity
mazda.Drive = "FWD";
mazda = await repository.UpdateAsync(mazda.Id, mazda);

// Delete entity
await repository.DeleteAsync(mazda2.Id);

Console.WriteLine("\nAll cars:");
allCars = await repository.GetAllAsync();
PrintCars(allCars);

void PrintCars(IEnumerable<CarDbo> cars)
{
    foreach (var car in cars)
    {
        Console.WriteLine($"{JsonSerializer.Serialize(car)}");
    }
}