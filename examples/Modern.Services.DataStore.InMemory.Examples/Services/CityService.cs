using Microsoft.Extensions.Logging;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;
using Modern.Services.DataStore.InMemory.Examples.Entities;
using Modern.Services.DataStore.InMemory.Examples.Models;
using Modern.Services.DataStore.InMemory.Examples.Repositories;

namespace Modern.Services.DataStore.InMemory.Examples.Services;

public class CityInMemoryService : ModernInMemoryService<CityDto, CityDbo, int, ICityRepository>, ICityInMemoryService
{
    public CityInMemoryService(ICityRepository repository, IModernServiceCache<CityDto, int> cache, ILogger<CityInMemoryService> logger)
        : base(repository, cache, logger)
    {
    }

    public async Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ToList().ConvertAll(MapToDto);
    }
}