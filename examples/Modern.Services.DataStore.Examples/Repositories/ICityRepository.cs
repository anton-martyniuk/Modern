using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.Examples.Entities;

namespace Modern.Services.DataStore.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<IEnumerable<CityDbo>> GetCountryCitiesAsync(string country);
}