using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Data.Repositories;

/// <summary>
/// WeatherRecord repository implementation
/// </summary>
public class WeatherRecordRepository : Repository<WeatherRecord>, IWeatherRecordRepository
{
    public WeatherRecordRepository(WeatherDbContext context) : base(context)
    {
    }

    public override async Task<WeatherRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.City)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<WeatherRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherRecord>> GetRecordsByCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.City)
            .Where(w => w.CityId == cityId)
            .OrderByDescending(w => w.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherRecord>> GetRecentRecordsAsync(int cityId, int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.City)
            .Where(w => w.CityId == cityId)
            .OrderByDescending(w => w.RecordedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherRecord?> GetLatestRecordForCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.City)
            .Where(w => w.CityId == cityId)
            .OrderByDescending(w => w.RecordedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
