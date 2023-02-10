using Modern.Services.DataStore.Abstractions;
using Modern.Services.DataStore.InMemory.Abstractions;
using Modern.Services.DataStore.InMemory.Examples.Entities;
using Modern.Services.DataStore.InMemory.Examples.Models;

namespace Modern.Services.DataStore.InMemory.Examples.Services;

public interface ICityService : IModernService<CityDto, CityDbo, int>
{
    Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country);
}

public interface ICityInMemoryService : IModernInMemoryService<CityDto, CityDbo, int>
{
    Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country);
}