using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modern.EfCore.Examples.DbContexts;

namespace Modern.EfCore.Examples.Migrations;

public class ToolsDbContextFactory: IDesignTimeDbContextFactory<CityDbContext>
{
    public CityDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;

        var optionsBuilder = new DbContextOptionsBuilder<CityDbContext>()
            .UseSqlite(connectionString);

        return new CityDbContext(optionsBuilder.Options);
    }
}