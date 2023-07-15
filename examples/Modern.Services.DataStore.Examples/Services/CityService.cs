using Microsoft.Extensions.Logging;
using Modern.Services.DataStore.Examples.Entities;
using Modern.Services.DataStore.Examples.Models;
using Modern.Services.DataStore.Examples.Repositories;

namespace Modern.Services.DataStore.Examples.Services;

public class CityService : ModernService<CityDto, CityDbo, int>, ICityService
{
    private readonly ICityRepository _repository;

    public CityService(ICityRepository repository, ILogger<CityService> logger)
        : base(repository, logger)
    {
        _repository = repository;
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await _repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}