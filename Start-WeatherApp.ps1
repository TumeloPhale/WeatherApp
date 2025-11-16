# Weather App Quick Start Script
# Run this script to start the Weather App API

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Weather App - Quick Start" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if in the correct directory
$currentDir = Get-Location
$expectedDir = "c:\dev\WeatherApp"

if ($currentDir.Path -ne $expectedDir) {
    Write-Host "Changing to project directory..." -ForegroundColor Yellow
    Set-Location $expectedDir
}

Write-Host "Building the solution..." -ForegroundColor Green
dotnet build --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build failed! Please check the errors above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Starting the Weather App API..." -ForegroundColor Green
Write-Host ""
Write-Host "The application will:" -ForegroundColor Yellow
Write-Host "  ✓ Create the database (if it doesn't exist)" -ForegroundColor White
Write-Host "  ✓ Run FluentMigrator migrations" -ForegroundColor White
Write-Host "  ✓ Seed initial data (cities, weather records, alerts)" -ForegroundColor White
Write-Host "  ✓ Start the API server" -ForegroundColor White
Write-Host ""
Write-Host "Once started, you can:" -ForegroundColor Yellow
Write-Host "  • View API docs at: http://localhost:5030/scalar/v1" -ForegroundColor Cyan
Write-Host "  • Check health at: http://localhost:5030/health" -ForegroundColor Cyan
Write-Host "  • Test endpoints using the Scalar UI" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to API project and run
Set-Location "WeatherApp.API"
dotnet run

# If the run fails or is interrupted
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Application stopped or encountered an error." -ForegroundColor Red
    Set-Location ..
    exit $LASTEXITCODE
}

Set-Location ..
