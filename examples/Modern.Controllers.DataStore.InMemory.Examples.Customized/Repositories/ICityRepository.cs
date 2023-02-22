using Modern.Controllers.DataStore.InMemory.Examples.Customized.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.Controllers.DataStore.InMemory.Examples.Customized.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}