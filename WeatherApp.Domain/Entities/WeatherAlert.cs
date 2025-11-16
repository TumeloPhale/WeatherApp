namespace WeatherApp.Domain.Entities;

/// <summary>
/// Represents a weather alert that can apply to multiple cities
/// </summary>
public class WeatherAlert
{
    public int Id { get; set; }
    
    public string AlertType { get; set; } = string.Empty;
    
    public string Severity { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property - Many-to-Many relationship with Cities
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
