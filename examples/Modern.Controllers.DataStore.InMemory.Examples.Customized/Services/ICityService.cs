using Modern.Controllers.DataStore.InMemory.Examples.Customized.Entities;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Models;
using Modern.Services.DataStore.InMemory.Abstractions;

namespace Modern.Controllers.DataStore.InMemory.Examples.Customized.Services;

public interface ICityInMemoryService : IModernInMemoryService<CityDto, CityDbo, int>
{
    Task<List<CityDto>> GetCountryCitiesAsync(string country);
}