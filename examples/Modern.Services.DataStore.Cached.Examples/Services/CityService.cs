using Microsoft.Extensions.Logging;
using Modern.Cache.Abstractions;
using Modern.Services.DataStore.Cached.Examples.Entities;
using Modern.Services.DataStore.Cached.Examples.Models;
using Modern.Services.DataStore.Cached.Examples.Repositories;

namespace Modern.Services.DataStore.Cached.Examples.Services;

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

public class CityCachedService : ModernCachedService<CityDto, CityDbo, int>, ICityService
{
    private readonly ICityService _service;

    public CityCachedService(ICityService service, IModernCache<CityDto, int> cache, ILogger<CityCachedService> logger)
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