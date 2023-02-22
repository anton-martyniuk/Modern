using Modern.Controllers.DataStore.Examples.Customized.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.Controllers.DataStore.Examples.Customized.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}