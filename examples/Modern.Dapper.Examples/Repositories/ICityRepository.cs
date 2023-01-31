using Modern.Dapper.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.Dapper.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<IEnumerable<CityDbo>> GetCountryCitiesAsync(string country);
}