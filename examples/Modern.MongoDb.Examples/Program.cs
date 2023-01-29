using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.Extensions.Microsoft.DependencyInjection;
using Modern.MongoDb.Examples.Entities;
using Modern.MongoDb.Examples.Repositories;

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
        options.AddConcreteRepository<IProductRepository, ProductRepository>();
    });

var provider = services.BuildServiceProvider();
var repository = provider.GetRequiredService<IProductRepository>();

var newProducts = new List<ProductDbo>
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
newProducts = await repository.CreateAsync(newProducts);

// Get ordered entities
Console.WriteLine("All products (price asc):");
var allProducts = await repository.OrderByPriceAsync(true);
Print(allProducts);

var awesomeProduct = newProducts.Last();
awesomeProduct.Attributes["Color"] = "White";

// Update entity
await repository.UpdateAsync(awesomeProduct.Id, awesomeProduct);

// Get filtered entities
Console.WriteLine("\nWhite products:");
var whiteProducts = await repository.FilterByAttributeAsync("Color", "White");
Print(whiteProducts);

// Delete entity
await repository.DeleteAsync(awesomeProduct.Id);

// Get ordered entities
Console.WriteLine("\nAll products (price desc):");
allProducts = await repository.OrderByPriceAsync(false);
Print(allProducts);

void Print(IEnumerable<ProductDbo> products)
{
    foreach (var product in products)
    {
        Console.WriteLine($"{JsonSerializer.Serialize(product)}");
    }
}