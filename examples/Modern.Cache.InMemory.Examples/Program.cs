﻿using System.Text.Json;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Modern.Cache.Abstractions;
using Modern.Cache.InMemory.Examples.Models;
using Modern.Extensions.Microsoft.DependencyInjection;

var services = new ServiceCollection();

// Add modern stuff
services
    .AddModern()
    .AddInMemoryCache(options =>
    {
        options.AddCache<Product, int>();
        options.CacheSettings.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
        options.CacheSettings.SlidingExpiration = TimeSpan.FromSeconds(10);
    });

var provider = services.BuildServiceProvider();

// Get concrete repository
var cache = provider.GetRequiredService<IModernCache<Product, int>>();

// Create entities
var newEntities = GetEntities(10).ToDictionary(key => key.Id, value => value);
await cache.AddOrUpdateAsync(newEntities);

// Find an entity
var firstEntity = newEntities.First();
var entity = await cache.GetByIdAsync(firstEntity.Key);

Console.WriteLine("Entity:");
Print(entity);

// Delete entity
await cache.DeleteAsync(firstEntity.Key);

entity = await cache.TryGetByIdAsync(firstEntity.Key);
Console.WriteLine("Entity was deleted: {0}", entity is null);

// ================================================================ END

void Print(Product entity)
{
    Console.WriteLine($"{JsonSerializer.Serialize(entity)}");
}

List<Product> GetEntities(int count)
{
    return new Faker<Product>()
        .RuleFor(x => x.Id, f => f.IndexFaker + 1)
        .RuleFor(x => x.Name, f => f.Address.City())
        .RuleFor(x => x.Price, f => f.Random.Decimal() * 100)
        .RuleFor(x => x.Quantity, f => f.Random.Int(0, 100))
        .Generate(count);
}