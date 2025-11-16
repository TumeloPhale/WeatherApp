using Microsoft.Extensions.Logging;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Services.DTOs;

namespace WeatherApp.Services;

public interface IWeatherRecordService
{
    Task<IEnumerable<WeatherRecordDto>> GetAllRecordsAsync(CancellationToken cancellationToken = default);
    Task<WeatherRecordDto?> GetRecordByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherRecordDto>> GetRecordsByCityAsync(int cityId, CancellationToken cancellationToken = default);
    Task<WeatherRecordDto?> GetLatestRecordForCityAsync(int cityId, CancellationToken cancellationToken = default);
    Task<WeatherRecordDto> CreateWeatherRecordAsync(CreateWeatherRecordDto recordDto, CancellationToken cancellationToken = default);
}

public class WeatherRecordService : IWeatherRecordService
{
    private readonly IWeatherRecordRepository _weatherRecordRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<WeatherRecordService> _logger;

    public WeatherRecordService(
        IWeatherRecordRepository weatherRecordRepository,
        ICityRepository cityRepository,
        ILogger<WeatherRecordService> logger)
    {
        _weatherRecordRepository = weatherRecordRepository;
        _cityRepository = cityRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherRecordDto>> GetAllRecordsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all weather records");
        
        var records = await _weatherRecordRepository.GetAllAsync(cancellationToken);
        return records.Select(MapToDto);
    }

    public async Task<WeatherRecordDto?> GetRecordByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving weather record with ID: {RecordId}", id);
        
        var record = await _weatherRecordRepository.GetByIdAsync(id, cancellationToken);
        return record != null ? MapToDto(record) : null;
    }

    public async Task<IEnumerable<WeatherRecordDto>> GetRecordsByCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving weather records for city ID: {CityId}", cityId);
        
        var records = await _weatherRecordRepository.GetRecordsByCityAsync(cityId, cancellationToken);
        return records.Select(MapToDto);
    }

    public async Task<WeatherRecordDto?> GetLatestRecordForCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving latest weather record for city ID: {CityId}", cityId);
        
        var record = await _weatherRecordRepository.GetLatestRecordForCityAsync(cityId, cancellationToken);
        return record != null ? MapToDto(record) : null;
    }

    public async Task<WeatherRecordDto> CreateWeatherRecordAsync(CreateWeatherRecordDto recordDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new weather record for city ID: {CityId}", recordDto.CityId);
        
        // Validation: Verify city exists
        var city = await _cityRepository.GetByIdAsync(recordDto.CityId, cancellationToken);
        if (city == null)
        {
            _logger.LogWarning("City with ID {CityId} not found", recordDto.CityId);
            throw new ArgumentException($"City with ID {recordDto.CityId} does not exist.");
        }

        // Validation: Temperature range (-100 to 100 Celsius is reasonable)
        if (recordDto.Temperature < -100 || recordDto.Temperature > 100)
        {
            throw new ArgumentException("Temperature must be between -100 and 100 degrees Celsius.");
        }

        // Validation: Humidity (0-100%)
        if (recordDto.Humidity < 0 || recordDto.Humidity > 100)
        {
            throw new ArgumentException("Humidity must be between 0 and 100 percent.");
        }

        // Validation: Wind speed (0-500 km/h is reasonable)
        if (recordDto.WindSpeed < 0 || recordDto.WindSpeed > 500)
        {
            throw new ArgumentException("Wind speed must be between 0 and 500 km/h.");
        }

        var record = new WeatherRecord
        {
            CityId = recordDto.CityId,
            Temperature = recordDto.Temperature,
            Humidity = recordDto.Humidity,
            WindSpeed = recordDto.WindSpeed,
            Description = recordDto.Description?.Trim(),
            RecordedAt = recordDto.RecordedAt ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            City = city
        };

        var createdRecord = await _weatherRecordRepository.AddAsync(record, cancellationToken);
        _logger.LogInformation("Weather record created successfully with ID: {RecordId}", createdRecord.Id);
        
        return MapToDto(createdRecord);
    }

    private static WeatherRecordDto MapToDto(WeatherRecord record)
    {
        return new WeatherRecordDto
        {
            Id = record.Id,
            CityId = record.CityId,
            CityName = record.City?.Name ?? string.Empty,
            Temperature = record.Temperature,
            Humidity = record.Humidity,
            WindSpeed = record.WindSpeed,
            Description = record.Description,
            RecordedAt = record.RecordedAt
        };
    }
}
