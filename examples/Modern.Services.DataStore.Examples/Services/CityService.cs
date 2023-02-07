using Microsoft.Extensions.Logging;
using Modern.Services.DataStore.Examples.Entities;
using Modern.Services.DataStore.Examples.Models;
using Modern.Services.DataStore.Examples.Repositories;

namespace Modern.Services.DataStore.Examples.Services;

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