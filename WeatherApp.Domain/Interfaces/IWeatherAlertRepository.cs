using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Interfaces;

/// <summary>
/// Specific repository interface for WeatherAlert operations
/// </summary>
public interface IWeatherAlertRepository : IRepository<WeatherAlert>
{
    Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WeatherAlert>> GetAlertsByCityAsync(int cityId, CancellationToken cancellationToken = default);
    
    Task AddAlertToCityAsync(int alertId, int cityId, CancellationToken cancellationToken = default);
}
