using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modern.Repositories.EFCore;
using Modern.Repositories.EFCore.Configuration;
using Modern.Services.DataStore.InMemory.Examples.DbContexts;
using Modern.Services.DataStore.InMemory.Examples.Entities;

namespace Modern.Services.DataStore.InMemory.Examples.Repositories;

public class CityRepository : ModernEfCoreRepositoryWithFactory<CityDbContext, CityDbo, int>, ICityRepository
{
    public CityRepository(IDbContextFactory<CityDbContext> dbContextFactory, IOptions<EfCoreRepositoryConfiguration> configuration)
        : base(dbContextFactory, configuration)
    {
    }

    public async Task<IEnumerable<CityDbo>> GetCountryCitiesAsync(string country)
    {
        await using var dbConnection = await DbContextFactory.CreateDbContextAsync();

        var entities = await dbConnection.Cities.Where(x => x.Country.Equals(country)).ToListAsync();
        return entities;
    }
}