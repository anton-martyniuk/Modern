using Modern.EfCore.Examples.Entities;
using Modern.Repositories.EFCore.Specifications.Base;

namespace Modern.EfCore.Examples.Specifications;

public class BigCitiesSpecification : Specification<CityDbo>
{
    public BigCitiesSpecification()
    {
        AddFilteringQuery(dbo => dbo.Area > 50_000 && dbo.Population > 500_000);
        AddOrderByQuery(dbo => dbo.Population);
        AddOrderByQuery(dbo => dbo.Area);
    }
}