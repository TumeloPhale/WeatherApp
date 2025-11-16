using WeatherApp.Domain.Entities;

namespace WeatherApp.Services.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCityDto
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class WeatherRecordDto
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public decimal Temperature { get; set; }
    public decimal Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public string? Description { get; set; }
    public DateTime RecordedAt { get; set; }
}

public class CreateWeatherRecordDto
{
    public int CityId { get; set; }
    public decimal Temperature { get; set; }
    public decimal Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public string? Description { get; set; }
    public DateTime? RecordedAt { get; set; }
}

public class WeatherAlertDto
{
    public int Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive { get; set; }
    public List<string> AffectedCities { get; set; } = new();
}

public class CreateWeatherAlertDto
{
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<int> CityIds { get; set; } = new();
}
