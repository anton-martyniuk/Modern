using Microsoft.Extensions.Logging;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;
using Modern.Services.DataStore.InMemory.Examples.Entities;
using Modern.Services.DataStore.InMemory.Examples.Models;
using Modern.Services.DataStore.InMemory.Examples.Repositories;

namespace Modern.Services.DataStore.InMemory.Examples.Services;

public class CityService : ModernService<CityDto, CityDbo, int, ICityRepository>, ICityService
{
    public CityService(ICityRepository repository, ILogger<CityService> logger)
        : base(repository, logger)
    {
    }

    public async Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.Select(MapToDto).ToList();
    }
}

public class CityInMemoryService : ModernInMemoryService<CityDto, CityDbo, int>, ICityInMemoryService
{
    private readonly ICityService _service;

    public CityInMemoryService(ICityService service, IModernServiceCache<CityDto, int> cache, ILogger<CityInMemoryService> logger)
        : base(service, cache, logger)
    {
        _service = service;
    }

    public async Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDto = await _service.GetCountryCitiesAsync(country);
        return entitiesDto;
    }
}