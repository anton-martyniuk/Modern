using Modern.MongoDb.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.MongoDb.Examples.Repositories;

public interface IProductRepository : IModernRepository<ProductDbo, string>
{
    Task<IEnumerable<ProductDbo>> FilterByAttributeAsync(string attributeName, string attributeValue);
    
    Task<IEnumerable<ProductDbo>> OrderByPriceAsync(bool ascending);
}