using LiteDB;
using Modern.LiteDb.Examples.Entities;
using Modern.Repositories.LiteDB;

namespace Modern.LiteDb.Examples.Repositories;

public class CarRepository : ModernLiteDbRepository<CarDbo, long>, ICarRepository
{
    private const string ConnectionString = "example_lite.db";
    private const string CollectionName = "cars";
    
    public CarRepository()
        : base(ConnectionString, CollectionName)
    {
    }

    public Task<List<CarDbo>> FindCarsByDriveTypeAsync(string drive)
    {
        using var db = new LiteDatabase(ConnectionString);
        var collection = db.GetCollection<CarDbo>(CollectionName);
        
        var cars = collection.Find(x => x.Drive.Equals(drive)).ToList();
        return Task.FromResult(cars);
    }
}