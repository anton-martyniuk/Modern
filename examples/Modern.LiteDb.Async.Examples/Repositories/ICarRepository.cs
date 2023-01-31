using Modern.LiteDb.Async.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.LiteDb.Async.Examples.Repositories;

public interface ICarRepository : IModernRepository<CarDbo, long>
{
    Task<List<CarDbo>> FindCarsByDriveTypeAsync(string drive);
}