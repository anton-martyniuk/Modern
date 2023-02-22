using Modern.Controllers.DataStore.Examples.Customized.Entities;
using Modern.Controllers.DataStore.Examples.Customized.Models;
using Modern.Controllers.DataStore.Examples.Customized.Repositories;
using Modern.Services.DataStore;

namespace Modern.Controllers.DataStore.Examples.Customized.Services;

public class CityService : ModernService<CityDto, CityDbo, int, ICityRepository>, ICityService
{
    public CityService(ICityRepository repository, ILogger<CityService> logger)
        : base(repository, logger)
    {
    }

    public async Task<List<CityDto>> GetCountryCitiesAsync(string country)
    {
        var entitiesDbo = await Repository.GetCountryCitiesAsync(country);
        return entitiesDbo.ConvertAll(MapToDto);
    }
}