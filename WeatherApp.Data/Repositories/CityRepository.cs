using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Data.Repositories;

/// <summary>
/// City repository implementation
/// </summary>
public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(WeatherDbContext context) : base(context)
    {
    }

    public async Task<City?> GetCityWithWeatherRecordsAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.WeatherRecords.OrderByDescending(w => w.RecordedAt))
            .FirstOrDefaultAsync(c => c.Id == cityId, cancellationToken);
    }

    public async Task<City?> GetCityByNameAsync(string cityName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<City>> GetCitiesWithActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.WeatherAlerts.Where(a => a.IsActive))
            .Where(c => c.WeatherAlerts.Any(a => a.IsActive))
            .ToListAsync(cancellationToken);
    }
}
