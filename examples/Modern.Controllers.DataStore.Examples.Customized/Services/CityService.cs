using Modern.Controllers.DataStore.Examples.Customized.Entities;
using Modern.Controllers.DataStore.Examples.Customized.Models;
using Modern.Controllers.DataStore.Examples.Customized.Repositories;
using Modern.Services.DataStore;

namespace Modern.Controllers.DataStore.Examples.Customized.Services;

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