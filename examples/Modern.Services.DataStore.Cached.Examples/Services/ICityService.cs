using Modern.Services.DataStore.Abstractions;
using Modern.Services.DataStore.Cached.Examples.Entities;
using Modern.Services.DataStore.Cached.Examples.Models;

namespace Modern.Services.DataStore.Cached.Examples.Services;

public interface ICityService : IModernService<CityDto, CityDbo, int>
{
    Task<List<CityDto>> GetCountryCitiesAsync(string country);
}