using Microsoft.Extensions.Options;
using Modern.Cache.Abstractions;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Entities;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Models;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Repositories;
using Modern.Services.DataStore.Cached;
using Modern.Services.DataStore.Cached.Configuration;

namespace Modern.Controllers.DataStore.Cached.Examples.Customized.Services;

public class CityCachedService : ModernCachedService<CityDto, CityDbo, int, ICityRepository>, ICityService
{
    public CityCachedService(ICityRepository repository, IModernCache<CityDto, int> cache,
        IOptions<ModernCachedServiceConfiguration> options, ILogger<CityCachedService> logger)
        : base(repository, cache, options, logger)
    {
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}