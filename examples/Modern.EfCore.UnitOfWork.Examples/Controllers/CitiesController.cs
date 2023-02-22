using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.DataStore;
using Modern.EfCore.UnitOfWork.Examples.Entities;
using Modern.EfCore.UnitOfWork.Examples.Models;
using Modern.EfCore.UnitOfWork.Examples.Services;

namespace Modern.EfCore.UnitOfWork.Examples.Controllers;

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
    [HttpGet("update-by-country/{country}")]
    public virtual async Task<IActionResult> GetCountryCities([Required] string country)
    {
        var entities = await _service.UpdateCountryCitiesAsync(country).ConfigureAwait(false);
        return Ok(entities);
    }
}