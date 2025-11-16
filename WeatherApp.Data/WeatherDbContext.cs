using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Data;

/// <summary>
/// Database context for the Weather Application
/// </summary>
public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<WeatherRecord> WeatherRecords { get; set; } = null!;
    public DbSet<WeatherAlert> WeatherAlerts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure City entity
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities");
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(c => c.Country)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(c => c.Latitude)
                .HasPrecision(9, 6);
            
            entity.Property(c => c.Longitude)
                .HasPrecision(9, 6);
            
            entity.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Create unique index on Name + Country
            entity.HasIndex(c => new { c.Name, c.Country })
                .IsUnique();
        });

        // Configure WeatherRecord entity
        modelBuilder.Entity<WeatherRecord>(entity =>
        {
            entity.ToTable("WeatherRecords");
            entity.HasKey(w => w.Id);
            
            entity.Property(w => w.Temperature)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(w => w.Humidity)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(w => w.WindSpeed)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(w => w.Description)
                .HasMaxLength(500);
            
            entity.Property(w => w.RecordedAt)
                .IsRequired();
            
            entity.Property(w => w.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Configure one-to-many relationship
            entity.HasOne(w => w.City)
                .WithMany(c => c.WeatherRecords)
                .HasForeignKey(w => w.CityId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Create index on CityId and RecordedAt for efficient queries
            entity.HasIndex(w => new { w.CityId, w.RecordedAt });
        });

        // Configure WeatherAlert entity
        modelBuilder.Entity<WeatherAlert>(entity =>
        {
            entity.ToTable("WeatherAlerts");
            entity.HasKey(a => a.Id);
            
            entity.Property(a => a.AlertType)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(a => a.Severity)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(a => a.StartTime)
                .IsRequired();
            
            entity.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Configure many-to-many relationship
            entity.HasMany(a => a.Cities)
                .WithMany(c => c.WeatherAlerts)
                .UsingEntity<Dictionary<string, object>>(
                    "CityWeatherAlert",
                    j => j.HasOne<City>().WithMany().HasForeignKey("CityId"),
                    j => j.HasOne<WeatherAlert>().WithMany().HasForeignKey("WeatherAlertId")
                );
        });
    }
}
