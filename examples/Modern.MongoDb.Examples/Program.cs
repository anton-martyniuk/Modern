using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.MongoDb.Examples.Entities;
using Modern.MongoDb.Examples.Repositories;
using Modern.Repositories.Abstractions;

var services = new ServiceCollection();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;

services
    .AddModern()
    .AddRepositoriesMongoDb(options =>
    {
        options.ConfigureMongoDbClient(connectionString!);

        // Add repository by entity type.
        // Use it when an own repository is NOT needed
        options.AddRepository<ProductDbo, string>("commercial", "products");
        
        // Add concrete repository inherited from IModernRepository
        // Use it when an own repository with specific methods is needed
        options.AddConcreteRepository<IProductRepository, ProductRepository>();
    });

var provider = services.BuildServiceProvider();

// Get repository by entity type
var repositoryByEntityType = provider.GetRequiredService<IModernRepository<ProductDbo, string>>();
var entities = await repositoryByEntityType.GetAllAsync();

// Get concrete repository
var repository = provider.GetRequiredService<IProductRepository>();

var newEntities = new List<ProductDbo>
{
    new()
    {
        Name = "Vacuum cleaner",
        Price = 199.99m,
        Quantity = 5,
        Attributes = new Dictionary<string, string>
        {
            ["Power"] = "2000W"
        }
    },
    new()
    {
        Name = "Mobile phone",
        Price = 710.00m,
        Quantity = 2,
        Attributes = new Dictionary<string, string>
        {
            ["Color"] = "White",
            ["RAM"] = "8 GB"
        }
    },
    new()
    {
        Name = "Awesome product",
        Price = 70.99m,
        Quantity = 7,
        Attributes = new Dictionary<string, string>
        {
            ["Color"] = "Fancy blue",
            ["Material"] = "Soft",
            ["Secret attribute"] = "Secret value"
        }
    }
};

// Create entities
newEntities = await repository.CreateAsync(newEntities);

// Get ordered entities
Console.WriteLine("All products (price asc):");
var allEntities = await repository.OrderByPriceAsync(true);
Print(allEntities);

var lastEntity = newEntities.Last();
lastEntity.Attributes["Color"] = "White";

// Update entity
await repository.UpdateAsync(lastEntity.Id, lastEntity);

// Get filtered entities
Console.WriteLine("\nWhite products:");
var whiteProducts = await repository.FilterByAttributeAsync("Color", "White");
Print(whiteProducts);

// Delete entity
await repository.DeleteAsync(lastEntity.Id);

// Get ordered entities
Console.WriteLine("\nAll products (price desc):");
allEntities = await repository.OrderByPriceAsync(false);
Print(allEntities);

void Print(IEnumerable<ProductDbo> products)
{
    foreach (var product in products)
    {
        Console.WriteLine($"{JsonSerializer.Serialize(product)}");
    }
}