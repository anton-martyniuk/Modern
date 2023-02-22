using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.DbContexts;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Entities;
using Modern.Repositories.EFCore;
using Modern.Repositories.EFCore.Configuration;

namespace Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Repositories;

public class CityRepository : ModernEfCoreRepositoryWithFactory<CityDbContext, CityDbo, int>, ICityRepository
{
    public CityRepository(IDbContextFactory<CityDbContext> dbContextFactory, IOptions<EfCoreRepositoryConfiguration> configuration)
        : base(dbContextFactory, configuration)
    {
    }

    public async Task<List<CityDbo>> GetCountryCitiesAsync(string country)
    {
        await using var dbConnection = await DbContextFactory.CreateDbContextAsync();

        var entities = await dbConnection.Cities.Where(x => x.Country.Equals(country)).ToListAsync();
        return entities;
    }
}