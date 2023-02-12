using Modern.Controllers.DataStore.Cached.Examples.Customized.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.Controllers.DataStore.Cached.Examples.Customized.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<IEnumerable<CityDbo>> GetCountryCitiesAsync(string country);
}