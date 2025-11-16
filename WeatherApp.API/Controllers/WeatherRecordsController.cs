using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using WeatherApp.Services.DTOs;

namespace WeatherApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherRecordsController : ControllerBase
{
    private readonly IWeatherRecordService _weatherRecordService;
    private readonly ILogger<WeatherRecordsController> _logger;

    public WeatherRecordsController(IWeatherRecordService weatherRecordService, ILogger<WeatherRecordsController> logger)
    {
        _weatherRecordService = weatherRecordService;
        _logger = logger;
    }

    /// <summary>
    /// Get all weather records
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherRecordDto>>> GetAllRecords(CancellationToken cancellationToken)
    {
        var records = await _weatherRecordService.GetAllRecordsAsync(cancellationToken);
        return Ok(records);
    }

    /// <summary>
    /// Get weather record by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherRecordDto>> GetRecordById(int id, CancellationToken cancellationToken)
    {
        var record = await _weatherRecordService.GetRecordByIdAsync(id, cancellationToken);
        
        if (record == null)
        {
            return NotFound(new { message = $"Weather record with ID {id} not found" });
        }

        return Ok(record);
    }

    /// <summary>
    /// Get all weather records for a specific city
    /// </summary>
    [HttpGet("city/{cityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherRecordDto>>> GetRecordsByCity(int cityId, CancellationToken cancellationToken)
    {
        var records = await _weatherRecordService.GetRecordsByCityAsync(cityId, cancellationToken);
        return Ok(records);
    }

    /// <summary>
    /// Get the latest weather record for a specific city
    /// </summary>
    [HttpGet("city/{cityId}/latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherRecordDto>> GetLatestRecordForCity(int cityId, CancellationToken cancellationToken)
    {
        var record = await _weatherRecordService.GetLatestRecordForCityAsync(cityId, cancellationToken);
        
        if (record == null)
        {
            return NotFound(new { message = $"No weather records found for city ID {cityId}" });
        }

        return Ok(record);
    }

    /// <summary>
    /// Create a new weather record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherRecordDto>> CreateWeatherRecord([FromBody] CreateWeatherRecordDto recordDto, CancellationToken cancellationToken)
    {
        try
        {
            var record = await _weatherRecordService.CreateWeatherRecordAsync(recordDto, cancellationToken);
            return CreatedAtAction(nameof(GetRecordById), new { id = record.Id }, record);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
