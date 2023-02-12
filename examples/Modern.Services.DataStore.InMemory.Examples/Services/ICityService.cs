using Modern.Services.DataStore.InMemory.Abstractions;
using Modern.Services.DataStore.InMemory.Examples.Entities;
using Modern.Services.DataStore.InMemory.Examples.Models;

namespace Modern.Services.DataStore.InMemory.Examples.Services;

public interface ICityInMemoryService : IModernInMemoryService<CityDto, CityDbo, int>
{
    Task<IEnumerable<CityDto>> GetCountryCitiesAsync(string country);
}