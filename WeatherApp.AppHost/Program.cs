var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server with database
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var weatherDb = sqlServer.AddDatabase("weatherdb");

// Add the Weather API with SQL Server database reference
var apiService = builder.AddProject("weatherapp-api", "../WeatherApp.API/WeatherApp.API.csproj")
    .WithReference(weatherDb)
    .WithExternalHttpEndpoints();

builder.Build().Run();
