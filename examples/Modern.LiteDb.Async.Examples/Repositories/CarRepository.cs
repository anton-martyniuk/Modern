using LiteDB.Async;
using Modern.LiteDb.Async.Examples.Entities;
using Modern.Repositories.LiteDB.Async;

namespace Modern.LiteDb.Async.Examples.Repositories;

public class CarRepository : ModernLiteDbAsyncRepository<CarDbo, long>, ICarRepository
{
    private const string ConnectionString = "example_lite.db";
    private const string CollectionName = "cars";
    
    public CarRepository()
        : base(ConnectionString, CollectionName)
    {
    }

    public async Task<List<CarDbo>> FindCarsByDriveTypeAsync(string drive)
    {
        using var db = new LiteDatabaseAsync(ConnectionString);
        var collection = db.GetCollection<CarDbo>(CollectionName);
        
        var cars = await collection.FindAsync(x => x.Drive.Equals(drive));
        return cars.ToList();
    }
}