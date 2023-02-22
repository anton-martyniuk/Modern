using Modern.EfCore.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.EfCore.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}