using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.LiteDb.Examples.Entities;
using Modern.LiteDb.Examples.Repositories;

var services = new ServiceCollection();

services
    .AddModern()
    .AddRepositoriesLiteDb(options =>
    {
        options.AddConcreteRepository<ICarRepository, CarRepository>();
    });

var provider = services.BuildServiceProvider();
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