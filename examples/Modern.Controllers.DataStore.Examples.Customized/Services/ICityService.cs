using Modern.Controllers.DataStore.Examples.Customized.Entities;
using Modern.Controllers.DataStore.Examples.Customized.Models;
using Modern.Services.DataStore.Abstractions;

namespace Modern.Controllers.DataStore.Examples.Customized.Services;

public interface ICityService : IModernService<CityDto, CityDbo, int>
{
    Task<List<CityDto>> GetCountryCitiesAsync(string country);
}