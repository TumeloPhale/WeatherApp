namespace WeatherApp.Domain.Entities;

/// <summary>
/// Represents a weather observation/record for a specific city at a specific time
/// </summary>
public class WeatherRecord
{
    public int Id { get; set; }
    
    public int CityId { get; set; }
    
    public decimal Temperature { get; set; }
    
    public decimal Humidity { get; set; }
    
    public decimal WindSpeed { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime RecordedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property - Many WeatherRecords belong to one City
    public virtual City City { get; set; } = null!;
}
