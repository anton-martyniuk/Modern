using Modern.Dapper.Examples.Entities;
using Modern.Repositories.Dapper.Mapping;

namespace Modern.Dapper.Examples.Mapping;

public class CityDboMapping : DapperEntityMapping<CityDbo>
{
    protected override void CreateMapping()
    {
        Table("cities")
            .Id(nameof(CityDbo.Id), "id")
            .Column(nameof(CityDbo.Name), "name")
            .Column(nameof(CityDbo.Country), "country")
            .Column(nameof(CityDbo.Area), "area")
            .Column(nameof(CityDbo.Population), "population")
            ;
    }
}