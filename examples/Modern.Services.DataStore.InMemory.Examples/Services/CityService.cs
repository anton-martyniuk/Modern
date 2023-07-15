using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;
using Modern.Services.DataStore.InMemory.Configuration;
using Modern.Services.DataStore.InMemory.Examples.Entities;
using Modern.Services.DataStore.InMemory.Examples.Models;
using Modern.Services.DataStore.InMemory.Examples.Repositories;

namespace Modern.Services.DataStore.InMemory.Examples.Services;

public class CityInMemoryService : ModernInMemoryService<CityDto, CityDbo, int>, ICityInMemoryService
{
    private readonly ICityRepository _repository;

    public CityInMemoryService(ICityRepository repository, IModernServiceCache<CityDto, int> cache,
        IOptions<ModernInMemoryServiceConfiguration> options,  ILogger<CityInMemoryService> logger)
        : base(repository, cache, options, logger)
    {
        _repository = repository;
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await _repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}