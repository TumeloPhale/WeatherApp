using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Interfaces;

/// <summary>
/// Specific repository interface for City operations
/// </summary>
public interface ICityRepository : IRepository<City>
{
    Task<City?> GetCityWithWeatherRecordsAsync(int cityId, CancellationToken cancellationToken = default);
    
    Task<City?> GetCityByNameAsync(string cityName, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<City>> GetCitiesWithActiveAlertsAsync(CancellationToken cancellationToken = default);
}
