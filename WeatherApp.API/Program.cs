using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Data.Repositories;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Services;
using FluentMigrator.Runner;
using System.Reflection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure database
ConfigureDatabase(builder);

// Register repositories and services
RegisterRepositories(builder.Services);
RegisterServices(builder.Services);

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<WeatherDbContext>();

var app = builder.Build();

// Initialize database
await InitializeDatabaseAsync(app);

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

app.Run();

// Helper methods
static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("weatherdb") 
        ?? builder.Configuration.GetConnectionString("DefaultConnection");
    
    builder.Services.AddDbContext<WeatherDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddSqlServer()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(Assembly.Load("WeatherApp.Migrations")).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());
}

static void RegisterRepositories(IServiceCollection services)
{
    services.AddScoped<ICityRepository, CityRepository>();
    services.AddScoped<IWeatherRecordRepository, WeatherRecordRepository>();
    services.AddScoped<IWeatherAlertRepository, WeatherAlertRepository>();
}

static void RegisterServices(IServiceCollection services)
{
    services.AddScoped<ICityService, CityService>();
    services.AddScoped<IWeatherRecordService, WeatherRecordService>();
    services.AddScoped<IWeatherAlertService, WeatherAlertService>();
}

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Test database connection
        var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        
        if (canConnect)
        {
            logger.LogInformation("Database connection established successfully.");
        }
        else
        {
            logger.LogWarning("Cannot connect to database. Attempting to create...");
            await dbContext.Database.MigrateAsync();
        }
        
        // Run FluentMigrator migrations
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        
        if (runner.HasMigrationsToApplyUp())
        {
            logger.LogInformation("Applying database migrations...");
            runner.MigrateUp();
            logger.LogInformation("Migrations completed successfully.");
        }
        else
        {
            logger.LogInformation("Database is up to date, no migrations needed.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database. Continuing startup...");
        // Don't throw - allow the app to start even if database initialization fails
    }
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();
    app.MapDefaultEndpoints();
}
