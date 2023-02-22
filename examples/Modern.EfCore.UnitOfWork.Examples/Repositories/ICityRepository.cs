using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.EfCore.UnitOfWork.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}