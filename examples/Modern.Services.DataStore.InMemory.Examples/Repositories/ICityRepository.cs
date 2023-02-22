using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.InMemory.Examples.Entities;

namespace Modern.Services.DataStore.InMemory.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}