using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using WeatherApp.Services.DTOs;

namespace WeatherApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherAlertsController : ControllerBase
{
    private readonly IWeatherAlertService _alertService;
    private readonly ILogger<WeatherAlertsController> _logger;

    public WeatherAlertsController(IWeatherAlertService alertService, ILogger<WeatherAlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    /// <summary>
    /// Get all weather alerts
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherAlertDto>>> GetAllAlerts(CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetAllAlertsAsync(cancellationToken);
        return Ok(alerts);
    }

    /// <summary>
    /// Get only active weather alerts
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherAlertDto>>> GetActiveAlerts(CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetActiveAlertsAsync(cancellationToken);
        return Ok(alerts);
    }

    /// <summary>
    /// Get weather alerts for a specific city
    /// </summary>
    [HttpGet("city/{cityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherAlertDto>>> GetAlertsByCity(int cityId, CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetAlertsByCityAsync(cityId, cancellationToken);
        return Ok(alerts);
    }

    /// <summary>
    /// Create a new weather alert
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherAlertDto>> CreateWeatherAlert([FromBody] CreateWeatherAlertDto alertDto, CancellationToken cancellationToken)
    {
        try
        {
            var alert = await _alertService.CreateWeatherAlertAsync(alertDto, cancellationToken);
            return CreatedAtAction(nameof(GetActiveAlerts), null, alert);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate a weather alert
    /// </summary>
    [HttpPut("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeactivateAlert(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _alertService.DeactivateAlertAsync(id, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
