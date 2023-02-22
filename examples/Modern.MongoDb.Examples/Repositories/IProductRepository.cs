using Modern.MongoDb.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.MongoDb.Examples.Repositories;

public interface IProductRepository : IModernRepository<ProductDbo, string>
{
    Task<List<ProductDbo>> FilterByAttributeAsync(string attributeName, string attributeValue);
    
    Task<List<ProductDbo>> OrderByPriceAsync(bool ascending);
}