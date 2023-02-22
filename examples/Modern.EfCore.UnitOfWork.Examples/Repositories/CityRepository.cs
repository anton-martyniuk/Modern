using Microsoft.EntityFrameworkCore;
using Modern.EfCore.UnitOfWork.Examples.DbContexts;
using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.Repositories.EFCore;

namespace Modern.EfCore.UnitOfWork.Examples.Repositories;

public class CityRepository : ModernEfCoreRepositoryForUnitOfWork<CityDbContext, CityDbo, int>, ICityRepository
{
    public CityRepository(CityDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<CityDbo>> GetCountryCitiesAsync(string country)
    {
        var entities = await DbContext.Cities.Where(x => x.Country.Equals(country)).ToListAsync();
        return entities;
    }
}