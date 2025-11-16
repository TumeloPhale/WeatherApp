using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using WeatherApp.Services.DTOs;

namespace WeatherApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ICityService cityService, ILogger<CitiesController> logger)
    {
        _cityService = cityService;
        _logger = logger;
    }

    /// <summary>
    /// Get all cities
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCities(CancellationToken cancellationToken)
    {
        var cities = await _cityService.GetAllCitiesAsync(cancellationToken);
        return Ok(cities);
    }

    /// <summary>
    /// Get city by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> GetCityById(int id, CancellationToken cancellationToken)
    {
        var city = await _cityService.GetCityByIdAsync(id, cancellationToken);
        
        if (city == null)
        {
            return NotFound(new { message = $"City with ID {id} not found" });
        }

        return Ok(city);
    }

    /// <summary>
    /// Create a new city
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CityDto>> CreateCity([FromBody] CreateCityDto cityDto, CancellationToken cancellationToken)
    {
        try
        {
            var city = await _cityService.CreateCityAsync(cityDto, cancellationToken);
            return CreatedAtAction(nameof(GetCityById), new { id = city.Id }, city);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get cities with active weather alerts
    /// </summary>
    [HttpGet("with-active-alerts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesWithActiveAlerts(CancellationToken cancellationToken)
    {
        var cities = await _cityService.GetCitiesWithActiveAlertsAsync(cancellationToken);
        return Ok(cities);
    }
}
