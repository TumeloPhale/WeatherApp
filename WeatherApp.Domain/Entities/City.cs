namespace WeatherApp.Domain.Entities;

/// <summary>
/// Represents a city where weather data is tracked
/// </summary>
public class City
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public decimal Latitude { get; set; }
    
    public decimal Longitude { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property - One City has many WeatherRecords
    public virtual ICollection<WeatherRecord> WeatherRecords { get; set; } = new List<WeatherRecord>();
    
    // Navigation property - Many-to-Many relationship with WeatherAlerts
    public virtual ICollection<WeatherAlert> WeatherAlerts { get; set; } = new List<WeatherAlert>();
}
