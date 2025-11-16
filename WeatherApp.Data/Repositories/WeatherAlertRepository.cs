using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Data.Repositories;

/// <summary>
/// WeatherAlert repository implementation
/// </summary>
public class WeatherAlertRepository : Repository<WeatherAlert>, IWeatherAlertRepository
{
    public WeatherAlertRepository(WeatherDbContext context) : base(context)
    {
    }

    public override async Task<WeatherAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Cities)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<WeatherAlert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Cities)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IsActive && (!a.EndTime.HasValue || a.EndTime > DateTime.UtcNow))
            .Include(a => a.Cities)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherAlert>> GetAlertsByCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Cities)
            .Where(a => a.Cities.Any(c => c.Id == cityId))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAlertToCityAsync(int alertId, int cityId, CancellationToken cancellationToken = default)
    {
        var alert = await _dbSet
            .Include(a => a.Cities)
            .FirstOrDefaultAsync(a => a.Id == alertId, cancellationToken);
        
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.Id == cityId, cancellationToken);

        if (alert != null && city != null && !alert.Cities.Contains(city))
        {
            alert.Cities.Add(city);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
