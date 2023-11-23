using Modern.MongoDb.Examples.Entities;
using Modern.Repositories.MongoDB.Specifications.Base;

namespace Modern.MongoDb.Examples.Specifications;

public class ExpensiveProductsSpecification : Specification<ProductDbo>
{
    public ExpensiveProductsSpecification()
    {
        AddFilteringQuery(dbo => dbo.Price > 500);
        AddOrderByQuery(dbo => dbo.Price);
        AddOrderByQuery(dbo => dbo.Name);
    }
}