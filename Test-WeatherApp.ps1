# WeatherApp API Test Script
# This script demonstrates the functionality of the Weather App API

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Weather App API Testing Script" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5030/api"

# Test 1: Get all cities
Write-Host "1. Getting all cities..." -ForegroundColor Yellow
$cities = Invoke-RestMethod -Uri "$baseUrl/cities" -Method Get
Write-Host "Found $($cities.Count) cities:" -ForegroundColor Green
$cities | Format-Table -Property id, name, country, latitude, longitude
Write-Host ""

# Test 2: Get weather records for London (CityId = 1)
Write-Host "2. Getting weather records for London..." -ForegroundColor Yellow
$records = Invoke-RestMethod -Uri "$baseUrl/weatherrecords/city/1" -Method Get
Write-Host "Found $($records.Count) weather records:" -ForegroundColor Green
$records | Format-Table -Property id, temperature, humidity, windSpeed, description, recordedAt
Write-Host ""

# Test 3: Get active weather alerts
Write-Host "3. Getting active weather alerts..." -ForegroundColor Yellow
$alerts = Invoke-RestMethod -Uri "$baseUrl/weatheralerts/active" -Method Get
Write-Host "Found $($alerts.Count) active alerts:" -ForegroundColor Green
$alerts | Format-Table -Property id, alertType, severity, description, @{Label="Cities"; Expression={$_.affectedCities -join ", "}}
Write-Host ""

# Test 4: Create a new city
Write-Host "4. Creating a new city (Berlin)..." -ForegroundColor Yellow
try {
    $newCity = @{
        name = "Berlin"
        country = "Germany"
        latitude = 52.5200
        longitude = 13.4050
    } | ConvertTo-Json

    $berlin = Invoke-RestMethod -Uri "$baseUrl/cities" -Method Post -Body $newCity -ContentType "application/json"
    Write-Host "City created successfully with ID: $($berlin.id)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "Error creating city: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 5: Create a weather record for the new city
Write-Host "5. Creating a weather record for Berlin..." -ForegroundColor Yellow
try {
    $berlinCity = Invoke-RestMethod -Uri "$baseUrl/cities" -Method Get | Where-Object { $_.name -eq "Berlin" }
    
    if ($berlinCity) {
        $newRecord = @{
            cityId = $berlinCity.id
            temperature = 18.5
            humidity = 72.0
            windSpeed = 8.5
            description = "Cloudy with occasional rain"
        } | ConvertTo-Json

        $record = Invoke-RestMethod -Uri "$baseUrl/weatherrecords" -Method Post -Body $newRecord -ContentType "application/json"
        Write-Host "Weather record created successfully with ID: $($record.id)" -ForegroundColor Green
        Write-Host ""
    }
} catch {
    Write-Host "Error creating weather record: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 6: Create a weather alert
Write-Host "6. Creating a weather alert for multiple cities..." -ForegroundColor Yellow
try {
    $newAlert = @{
        alertType = "Fog"
        severity = "Low"
        description = "Dense fog expected in the morning hours, visibility may be reduced"
        startTime = (Get-Date).ToUniversalTime().ToString("o")
        endTime = (Get-Date).AddHours(8).ToUniversalTime().ToString("o")
        cityIds = @(1, 5)  # London and Paris
    } | ConvertTo-Json

    $alert = Invoke-RestMethod -Uri "$baseUrl/weatheralerts" -Method Post -Body $newAlert -ContentType "application/json"
    Write-Host "Weather alert created successfully with ID: $($alert.id)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "Error creating alert: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 7: Get cities with active alerts
Write-Host "7. Getting cities with active alerts..." -ForegroundColor Yellow
$citiesWithAlerts = Invoke-RestMethod -Uri "$baseUrl/cities/with-active-alerts" -Method Get
Write-Host "Found $($citiesWithAlerts.Count) cities with active alerts:" -ForegroundColor Green
$citiesWithAlerts | Format-Table -Property id, name, country
Write-Host ""

# Test 8: Test validation - try to create duplicate city
Write-Host "8. Testing validation - attempting to create duplicate city..." -ForegroundColor Yellow
try {
    $duplicateCity = @{
        name = "London"
        country = "United Kingdom"
        latitude = 51.5074
        longitude = -0.1278
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$baseUrl/cities" -Method Post -Body $duplicateCity -ContentType "application/json"
    Write-Host "ERROR: Duplicate city was allowed!" -ForegroundColor Red
} catch {
    Write-Host "Validation working correctly - duplicate prevented: $($_.Exception.Message)" -ForegroundColor Green
}
Write-Host ""

# Test 9: Test validation - invalid temperature
Write-Host "9. Testing validation - attempting invalid temperature..." -ForegroundColor Yellow
try {
    $invalidRecord = @{
        cityId = 1
        temperature = 150.0  # Invalid - too high
        humidity = 50.0
        windSpeed = 10.0
        description = "Test"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$baseUrl/weatherrecords" -Method Post -Body $invalidRecord -ContentType "application/json"
    Write-Host "ERROR: Invalid temperature was allowed!" -ForegroundColor Red
} catch {
    Write-Host "Validation working correctly - invalid temperature prevented: $($_.Exception.Message)" -ForegroundColor Green
}
Write-Host ""

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "All tests completed!" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can also access the Swagger UI at: http://localhost:5030/swagger" -ForegroundColor Yellow
