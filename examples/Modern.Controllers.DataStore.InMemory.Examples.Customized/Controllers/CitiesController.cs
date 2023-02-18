using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Entities;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Models;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Services;

namespace Modern.Controllers.DataStore.InMemory.Examples.Customized.Controllers;

[ApiController]
[Route("cities")]
public class CitiesController : ModernInMemoryController<CreateCityRequest, UpdateCityRequest, CityDto, CityDbo, int>
{
    private readonly ICityInMemoryService _service;

    public CitiesController(ICityInMemoryService service) : base(service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Returns a list of cities that belong to the given <paramref name="country"/>
    /// </summary>
    /// <param name="country">The country name</param>
    /// <returns>A collection of cities</returns>
    [HttpGet("get-by-country/{country}")]
    public virtual async Task<IActionResult> GetCountryCities([Required] string country)
    {
        var entities = await _service.GetCountryCitiesAsync(country).ConfigureAwait(false);
        return Ok(entities);
    }
}