using Microsoft.Extensions.Options;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Entities;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Models;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Repositories;
using Modern.Services.DataStore.InMemory;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;
using Modern.Services.DataStore.InMemory.Configuration;

namespace Modern.Controllers.DataStore.InMemory.Examples.Customized.Services;

public class CityInMemoryService : ModernInMemoryService<CityDto, CityDbo, int, ICityRepository>, ICityInMemoryService
{
    public CityInMemoryService(ICityRepository repository, IModernServiceCache<CityDto, int> cache,
        IOptions<ModernInMemoryServiceConfiguration> options, ILogger<CityInMemoryService> logger)
        : base(repository, cache, options, logger)
    {
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}