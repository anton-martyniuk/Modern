using MediatR;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Entities;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Models;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Repositories;
using Modern.CQRS.DataStore.Cached.Abstract;

namespace Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.CQRS;

/// <summary>
/// Inherit from BaseMediatorHandler to use base methods like: MapToDto, MapToDbo, CreateProperException
/// </summary>
public class GetCountryCitiesQueryHandler : BaseMediatorHandler<CityDto, CityDbo, int>, IRequestHandler<GetCountryCitiesQuery, List<CityDto>>
{
    private readonly ICityRepository _cityRepository;

    public GetCountryCitiesQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<List<CityDto>> Handle(GetCountryCitiesQuery request, CancellationToken cancellationToken)
    {
        var cities = await _cityRepository.GetCountryCitiesAsync(request.Country);
        return cities.ConvertAll(MapToDto);
    }
}
