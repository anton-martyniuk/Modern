using Modern.LiteDb.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.LiteDb.Examples.Repositories;

public interface ICarRepository : IModernRepository<CarDbo, long>
{
    Task<List<CarDbo>> FindCarsByDriveTypeAsync(string drive);
}