using Microsoft.Extensions.Logging;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Services.DTOs;

namespace WeatherApp.Services;

public interface IWeatherAlertService
{
    Task<IEnumerable<WeatherAlertDto>> GetAllAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherAlertDto>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherAlertDto>> GetAlertsByCityAsync(int cityId, CancellationToken cancellationToken = default);
    Task<WeatherAlertDto> CreateWeatherAlertAsync(CreateWeatherAlertDto alertDto, CancellationToken cancellationToken = default);
    Task DeactivateAlertAsync(int alertId, CancellationToken cancellationToken = default);
}

public class WeatherAlertService : IWeatherAlertService
{
    private readonly IWeatherAlertRepository _alertRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<WeatherAlertService> _logger;

    private static readonly HashSet<string> ValidAlertTypes = new() 
    { 
        "Storm", "Heatwave", "Flood", "Snow", "Fog", "Wind", "Tornado", "Hurricane" 
    };

    private static readonly HashSet<string> ValidSeverities = new() 
    { 
        "Low", "Medium", "High", "Critical" 
    };

    public WeatherAlertService(
        IWeatherAlertRepository alertRepository,
        ICityRepository cityRepository,
        ILogger<WeatherAlertService> logger)
    {
        _alertRepository = alertRepository;
        _cityRepository = cityRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherAlertDto>> GetAllAlertsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all weather alerts");
        
        var alerts = await _alertRepository.GetAllAsync(cancellationToken);
        return alerts.Select(MapToDto);
    }

    public async Task<IEnumerable<WeatherAlertDto>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving active weather alerts");
        
        var alerts = await _alertRepository.GetActiveAlertsAsync(cancellationToken);
        return alerts.Select(MapToDto);
    }

    public async Task<IEnumerable<WeatherAlertDto>> GetAlertsByCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving weather alerts for city ID: {CityId}", cityId);
        
        var alerts = await _alertRepository.GetAlertsByCityAsync(cityId, cancellationToken);
        return alerts.Select(MapToDto);
    }

    public async Task<WeatherAlertDto> CreateWeatherAlertAsync(CreateWeatherAlertDto alertDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new weather alert of type: {AlertType}", alertDto.AlertType);
        
        // Validation: Alert type
        if (!ValidAlertTypes.Contains(alertDto.AlertType))
        {
            throw new ArgumentException($"Alert type must be one of: {string.Join(", ", ValidAlertTypes)}");
        }

        // Validation: Severity
        if (!ValidSeverities.Contains(alertDto.Severity))
        {
            throw new ArgumentException($"Severity must be one of: {string.Join(", ", ValidSeverities)}");
        }

        // Validation: Description length
        if (string.IsNullOrWhiteSpace(alertDto.Description) || alertDto.Description.Length < 10)
        {
            throw new ArgumentException("Description must be at least 10 characters long.");
        }

        // Validation: Time range
        if (alertDto.EndTime.HasValue && alertDto.EndTime <= alertDto.StartTime)
        {
            throw new ArgumentException("End time must be after start time.");
        }

        // Validation: Verify all cities exist
        foreach (var cityId in alertDto.CityIds)
        {
            var city = await _cityRepository.GetByIdAsync(cityId, cancellationToken);
            if (city == null)
            {
                _logger.LogWarning("City with ID {CityId} not found", cityId);
                throw new ArgumentException($"City with ID {cityId} does not exist.");
            }
        }

        var alert = new WeatherAlert
        {
            AlertType = alertDto.AlertType,
            Severity = alertDto.Severity,
            Description = alertDto.Description.Trim(),
            StartTime = alertDto.StartTime,
            EndTime = alertDto.EndTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdAlert = await _alertRepository.AddAsync(alert, cancellationToken);
        
        // Add associations to cities
        foreach (var cityId in alertDto.CityIds)
        {
            await _alertRepository.AddAlertToCityAsync(createdAlert.Id, cityId, cancellationToken);
        }

        _logger.LogInformation("Weather alert created successfully with ID: {AlertId}", createdAlert.Id);
        
        // Retrieve the alert with cities for proper DTO mapping
        var alertWithCities = await _alertRepository.GetByIdAsync(createdAlert.Id, cancellationToken);
        return MapToDto(alertWithCities!);
    }

    public async Task DeactivateAlertAsync(int alertId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating weather alert with ID: {AlertId}", alertId);
        
        var alert = await _alertRepository.GetByIdAsync(alertId, cancellationToken);
        if (alert == null)
        {
            _logger.LogWarning("Alert with ID {AlertId} not found", alertId);
            throw new ArgumentException($"Alert with ID {alertId} does not exist.");
        }

        alert.IsActive = false;
        alert.EndTime = DateTime.UtcNow;
        
        await _alertRepository.UpdateAsync(alert, cancellationToken);
        _logger.LogInformation("Weather alert deactivated successfully");
    }

    private static WeatherAlertDto MapToDto(WeatherAlert alert)
    {
        return new WeatherAlertDto
        {
            Id = alert.Id,
            AlertType = alert.AlertType,
            Severity = alert.Severity,
            Description = alert.Description,
            StartTime = alert.StartTime,
            EndTime = alert.EndTime,
            IsActive = alert.IsActive,
            AffectedCities = alert.Cities.Select(c => c.Name).ToList()
        };
    }
}
