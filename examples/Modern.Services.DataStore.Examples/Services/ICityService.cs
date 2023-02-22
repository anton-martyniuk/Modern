using Modern.Services.DataStore.Abstractions;
using Modern.Services.DataStore.Examples.Entities;
using Modern.Services.DataStore.Examples.Models;

namespace Modern.Services.DataStore.Examples.Services;

public interface ICityService : IModernService<CityDto, CityDbo, int>
{
    Task<List<CityDto>> GetCountryCitiesAsync(string country);
}