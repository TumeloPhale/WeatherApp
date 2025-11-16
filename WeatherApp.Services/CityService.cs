using Microsoft.Extensions.Logging;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Services.DTOs;

namespace WeatherApp.Services;

public interface ICityService
{
    Task<IEnumerable<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default);
    Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CityDto> CreateCityAsync(CreateCityDto cityDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<CityDto>> GetCitiesWithActiveAlertsAsync(CancellationToken cancellationToken = default);
}

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<CityService> _logger;

    public CityService(ICityRepository cityRepository, ILogger<CityService> logger)
    {
        _cityRepository = cityRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all cities");
        
        var cities = await _cityRepository.GetAllAsync(cancellationToken);
        return cities.Select(MapToDto);
    }

    public async Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving city with ID: {CityId}", id);
        
        var city = await _cityRepository.GetByIdAsync(id, cancellationToken);
        return city != null ? MapToDto(city) : null;
    }

    public async Task<CityDto> CreateCityAsync(CreateCityDto cityDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new city: {CityName}, {Country}", cityDto.Name, cityDto.Country);
        
        // Validation: Check for duplicate city
        var existingCity = await _cityRepository.GetCityByNameAsync(cityDto.Name, cancellationToken);
        if (existingCity != null && existingCity.Country.Equals(cityDto.Country, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("City {CityName} in {Country} already exists", cityDto.Name, cityDto.Country);
            throw new InvalidOperationException($"City '{cityDto.Name}' in '{cityDto.Country}' already exists.");
        }

        // Validation: Check latitude/longitude ranges
        if (cityDto.Latitude < -90 || cityDto.Latitude > 90)
        {
            throw new ArgumentException("Latitude must be between -90 and 90 degrees.");
        }

        if (cityDto.Longitude < -180 || cityDto.Longitude > 180)
        {
            throw new ArgumentException("Longitude must be between -180 and 180 degrees.");
        }

        var city = new City
        {
            Name = cityDto.Name.Trim(),
            Country = cityDto.Country.Trim(),
            Latitude = cityDto.Latitude,
            Longitude = cityDto.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        var createdCity = await _cityRepository.AddAsync(city, cancellationToken);
        _logger.LogInformation("City created successfully with ID: {CityId}", createdCity.Id);
        
        return MapToDto(createdCity);
    }

    public async Task<IEnumerable<CityDto>> GetCitiesWithActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving cities with active alerts");
        
        var cities = await _cityRepository.GetCitiesWithActiveAlertsAsync(cancellationToken);
        return cities.Select(MapToDto);
    }

    private static CityDto MapToDto(City city)
    {
        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            Country = city.Country,
            Latitude = city.Latitude,
            Longitude = city.Longitude,
            CreatedAt = city.CreatedAt
        };
    }
}
