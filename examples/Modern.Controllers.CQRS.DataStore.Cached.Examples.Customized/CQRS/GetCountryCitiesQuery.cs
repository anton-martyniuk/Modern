using MediatR;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Models;

namespace Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.CQRS;

public record GetCountryCitiesQuery(string Country) : IRequest<List<CityDto>>;
