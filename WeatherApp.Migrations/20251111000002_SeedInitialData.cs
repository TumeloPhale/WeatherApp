using FluentMigrator;

namespace WeatherApp.Migrations;

[Migration(20251111000002)]
public class SeedInitialData : Migration
{
    public override void Up()
    {
        // Seed Cities
        Insert.IntoTable("Cities").Row(new
        {
            Name = "London",
            Country = "United Kingdom",
            Latitude = 51.5074,
            Longitude = -0.1278,
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("Cities").Row(new
        {
            Name = "New York",
            Country = "United States",
            Latitude = 40.7128,
            Longitude = -74.0060,
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("Cities").Row(new
        {
            Name = "Tokyo",
            Country = "Japan",
            Latitude = 35.6762,
            Longitude = 139.6503,
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("Cities").Row(new
        {
            Name = "Sydney",
            Country = "Australia",
            Latitude = -33.8688,
            Longitude = 151.2093,
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("Cities").Row(new
        {
            Name = "Paris",
            Country = "France",
            Latitude = 48.8566,
            Longitude = 2.3522,
            CreatedAt = DateTime.UtcNow
        });

        // Seed Weather Records for London (CityId = 1)
        Insert.IntoTable("WeatherRecords").Row(new
        {
            CityId = 1,
            Temperature = 15.5,
            Humidity = 65.0,
            WindSpeed = 12.5,
            Description = "Partly cloudy",
            RecordedAt = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("WeatherRecords").Row(new
        {
            CityId = 1,
            Temperature = 14.8,
            Humidity = 68.0,
            WindSpeed = 15.0,
            Description = "Overcast",
            RecordedAt = DateTime.UtcNow.AddHours(-1),
            CreatedAt = DateTime.UtcNow
        });

        // Seed Weather Records for New York (CityId = 2)
        Insert.IntoTable("WeatherRecords").Row(new
        {
            CityId = 2,
            Temperature = 22.3,
            Humidity = 55.0,
            WindSpeed = 8.0,
            Description = "Clear sky",
            RecordedAt = DateTime.UtcNow.AddHours(-3),
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("WeatherRecords").Row(new
        {
            CityId = 2,
            Temperature = 23.1,
            Humidity = 52.0,
            WindSpeed = 9.5,
            Description = "Sunny",
            RecordedAt = DateTime.UtcNow.AddHours(-1),
            CreatedAt = DateTime.UtcNow
        });

        // Seed Weather Records for Tokyo (CityId = 3)
        Insert.IntoTable("WeatherRecords").Row(new
        {
            CityId = 3,
            Temperature = 18.7,
            Humidity = 70.0,
            WindSpeed = 5.5,
            Description = "Light rain",
            RecordedAt = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow
        });

        // Seed Weather Alerts
        Insert.IntoTable("WeatherAlerts").Row(new
        {
            AlertType = "Storm",
            Severity = "High",
            Description = "Severe thunderstorm warning with potential for heavy rain and strong winds",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(6),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        Insert.IntoTable("WeatherAlerts").Row(new
        {
            AlertType = "Heatwave",
            Severity = "Medium",
            Description = "High temperatures expected, stay hydrated and avoid prolonged sun exposure",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddDays(2),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Link alerts to cities (Many-to-Many)
        // Storm alert for London and Paris
        Insert.IntoTable("CityWeatherAlert").Row(new { CityId = 1, WeatherAlertId = 1 });
        Insert.IntoTable("CityWeatherAlert").Row(new { CityId = 5, WeatherAlertId = 1 });

        // Heatwave alert for Sydney
        Insert.IntoTable("CityWeatherAlert").Row(new { CityId = 4, WeatherAlertId = 2 });
    }

    public override void Down()
    {
        Delete.FromTable("CityWeatherAlert").AllRows();
        Delete.FromTable("WeatherRecords").AllRows();
        Delete.FromTable("WeatherAlerts").AllRows();
        Delete.FromTable("Cities").AllRows();
    }
}
