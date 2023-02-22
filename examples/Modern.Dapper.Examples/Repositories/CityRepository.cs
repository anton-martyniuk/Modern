using Dapper;
using Modern.Dapper.Examples.Entities;
using Modern.Dapper.Examples.Mapping;
using Modern.Repositories.Dapper;
using Modern.Repositories.Dapper.Connection;
using Modern.Repositories.Dapper.Providers;

namespace Modern.Dapper.Examples.Repositories;

public class CityRepository : ModernDapperRepository<CityDboMapping, CityDbo, int>, ICityRepository
{
    public CityRepository(IDbConnectionFactory connectionFactory, CityDboMapping mapping, IQueryProviderFactory factory)
        : base(connectionFactory, mapping, factory)
    {
    }

    public async Task<List<CityDbo>> GetCountryCitiesAsync(string country)
    {
        await using var dbConnection = DbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        const string columnsQuery = "id as Id, name as Name, country as Country, area as Area, population as Population";
        const string query = $"SELECT {columnsQuery} FROM cities WHERE country like '%@Country%'";
        
        var entities = await dbConnection.QueryAsync<CityDbo>(new CommandDefinition(query, new { Country = country }));
        return entities.ToList();
    }
}