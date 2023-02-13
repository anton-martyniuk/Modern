using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.DataStore.InMemory.OData.Examples.Models;
using Modern.Services.DataStore.InMemory.Abstractions.Cache;

namespace Modern.Controllers.DataStore.InMemory.OData.Examples.OData;

[ApiController]
[Route("api/odata/cities")]
public class CitiesODataController : ModernInMemoryODataController<CityDto, int>
{
    public CitiesODataController(IModernServiceCache<CityDto, int> cache) : base(cache)
    {
    }
}