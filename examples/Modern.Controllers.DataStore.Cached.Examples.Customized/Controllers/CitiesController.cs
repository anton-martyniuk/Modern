using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Entities;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Models;
using Modern.Controllers.DataStore.Cached.Examples.Customized.Services;

namespace Modern.Controllers.DataStore.Cached.Examples.Customized.Controllers;

[ApiController]
[Route("cities")]
public class CitiesController : ModernController<CreateCityRequest, UpdateCityRequest, CityDto, CityDbo, int>
{
    private readonly ICityService _service;

    public CitiesController(ICityService service) : base(service)
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
        var entity = await _service.GetCountryCitiesAsync(country).ConfigureAwait(false);
        return Ok(entity);
    }
}