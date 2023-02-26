using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modern.Cache.Abstractions;
using Modern.Services.DataStore.Cached.Configuration;
using Modern.Services.DataStore.Cached.Examples.Entities;
using Modern.Services.DataStore.Cached.Examples.Models;
using Modern.Services.DataStore.Cached.Examples.Repositories;

namespace Modern.Services.DataStore.Cached.Examples.Services;

public class CityCachedService : ModernCachedService<CityDto, CityDbo, int, ICityRepository>, ICityService
{
    public CityCachedService(ICityRepository repository, IModernCache<CityDto, int> cache,
        IOptions<ModernCachedServiceConfiguration> options,  ILogger<CityCachedService> logger)
        : base(repository, cache, options, logger)
    {
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}