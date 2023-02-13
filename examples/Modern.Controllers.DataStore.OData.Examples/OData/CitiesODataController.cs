using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.DataStore.OData.Examples.Entities;
using Modern.Repositories.Abstractions;

namespace Modern.Controllers.DataStore.OData.Examples.OData;

[ApiController]
[Route("api/odata/cities")]
public class CitiesODataController : ModernODataController<CityDbo, int>
{
    public CitiesODataController(IModernRepository<CityDbo, int> repository) : base(repository)
    {
    }
}