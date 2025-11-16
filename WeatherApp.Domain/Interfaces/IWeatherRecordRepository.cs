using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Interfaces;

/// <summary>
/// Specific repository interface for WeatherRecord operations
/// </summary>
public interface IWeatherRecordRepository : IRepository<WeatherRecord>
{
    Task<IEnumerable<WeatherRecord>> GetRecordsByCityAsync(int cityId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WeatherRecord>> GetRecentRecordsAsync(int cityId, int count, CancellationToken cancellationToken = default);
    
    Task<WeatherRecord?> GetLatestRecordForCityAsync(int cityId, CancellationToken cancellationToken = default);
}
