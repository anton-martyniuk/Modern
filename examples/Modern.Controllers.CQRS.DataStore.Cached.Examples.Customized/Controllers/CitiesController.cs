using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.CQRS;
using Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Models;

namespace Modern.Controllers.CQRS.DataStore.Cached.Examples.Customized.Controllers;

[ApiController]
[Route("cities")]
public class CitiesCqrsController : ModernCqrsController<CreateCityRequest, UpdateCityRequest, CityDto, int>
{
    private readonly IMediator _mediator;

    public CitiesCqrsController(IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Returns a list of cities that belong to the given <paramref name="country"/>
    /// </summary>
    /// <param name="country">The country name</param>
    /// <returns>A collection of cities</returns>
    [HttpGet("get-by-country/{country}")]
    public virtual async Task<IActionResult> GetCountryCities([Required] string country)
    {
        var entities = await _mediator.Send(new GetCountryCitiesQuery(country));
        return Ok(entities);
    }
}