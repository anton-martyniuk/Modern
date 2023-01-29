using Modern.MongoDb.Examples.Entities;
using Modern.Repositories.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Modern.MongoDb.Examples.Repositories;

public class ProductRepository : ModernMongoDbRepository<ProductDbo, string>, IProductRepository
{
    public ProductRepository(IMongoClient mongoClient)
        : base(mongoClient, "commercial", "products")
    {
    }

    public async Task<IEnumerable<ProductDbo>> FilterByAttributeAsync(string attributeName, string attributeValue)
    {
        // Use IModernRepository method
        return await WhereAsync(dbo => dbo.Attributes.Any(attr => attr.Key.Equals(attributeName) && attr.Value.Equals(attributeValue)));

        // Native approach
        var keyValuePair = new KeyValuePair<string, string>(attributeName, attributeValue);
        
        return await MongoCollection
            .Find(Builders<ProductDbo>.Filter.AnyEq(x => x.Attributes, keyValuePair))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductDbo>> OrderByPriceAsync(bool ascending)
    {
        if (ascending)
        {
            return await MongoCollection.AsQueryable().OrderBy(x => x.Price).ToListAsync();
        }

        return await MongoCollection.AsQueryable().OrderByDescending(x => x.Price).ToListAsync();
    }
}