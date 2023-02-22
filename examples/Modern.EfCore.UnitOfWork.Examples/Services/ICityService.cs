using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.EfCore.UnitOfWork.Examples.Models;
using Modern.Services.DataStore.Abstractions;

namespace Modern.EfCore.UnitOfWork.Examples.Services;

public interface ICityService : IModernService<CityDto, CityDbo, int>
{
    Task<List<CityDto>> UpdateCountryCitiesAsync(string country);
}