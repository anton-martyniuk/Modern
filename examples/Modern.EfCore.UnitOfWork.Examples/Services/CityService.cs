using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.EfCore.UnitOfWork.Examples.Models;
using Modern.EfCore.UnitOfWork.Examples.Repositories;
using Modern.Repositories.EFCore.UnitOfWork;
using Modern.Services.DataStore;

namespace Modern.EfCore.UnitOfWork.Examples.Services;

public class CityService : ModernService<CityDto, CityDbo, int>, ICityService
{
    private readonly IModernUnitOfWork _unitOfWork;
    private readonly ICityRepository _repository;

    public CityService(IModernUnitOfWork unitOfWork, ICityRepository repository, ILogger<CityService> logger)
        : base(repository, logger)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public override async Task<CityDto> CreateAsync(CityDto entity, CancellationToken cancellationToken = default)
    {
        var entityDbo = MapToDbo(entity);
        entityDbo = await Repository.CreateAsync(entityDbo, cancellationToken).ConfigureAwait(false);
        
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entityDbo);
    }

    public override async Task<List<CityDto>> CreateAsync(List<CityDto> entities, CancellationToken cancellationToken = default)
    {
        var entitiesDbo = entities.ConvertAll(MapToDbo);
        entitiesDbo = await Repository.CreateAsync(entitiesDbo, cancellationToken).ConfigureAwait(false);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return entitiesDbo.ConvertAll(MapToDto);
    }

    public async Task<List<CityDto>> UpdateCountryCitiesAsync(string country)
    {
        var entitiesDbo = await _repository.GetCountryCitiesAsync(country);
        foreach (var cityDbo in entitiesDbo)
        {
            cityDbo.Population += 1_000;
        }
        
        // This only adds entities into the ChangeTracker. SaveChanges is not executed here
        entitiesDbo = await Repository.UpdateAsync(entitiesDbo);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        return entitiesDbo.ConvertAll(MapToDto);
    }
}