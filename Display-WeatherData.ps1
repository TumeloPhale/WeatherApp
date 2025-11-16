# Weather App Data Display Script
# Shows all data in a nicely formatted way

$baseUrl = "http://localhost:5030/api"

Write-Host "`n╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     WEATHER APP - DATA DISPLAY         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Display Cities
Write-Host "📍 CITIES" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Yellow
$cities = Invoke-RestMethod -Uri "$baseUrl/cities" -Method Get
$cities | Format-Table -Property @{
    Label="ID"; Expression={$_.id}; Width=4
}, @{
    Label="City"; Expression={$_.name}; Width=15
}, @{
    Label="Country"; Expression={$_.country}; Width=18
}, @{
    Label="Latitude"; Expression={$_.latitude.ToString("F2")}; Width=10
}, @{
    Label="Longitude"; Expression={$_.longitude.ToString("F2")}; Width=10
} -AutoSize

Write-Host "`n🌡️  WEATHER RECORDS" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Yellow
$records = Invoke-RestMethod -Uri "$baseUrl/weatherrecords" -Method Get
$records | Format-Table -Property @{
    Label="City"; Expression={$_.cityName}; Width=15
}, @{
    Label="Temp (°C)"; Expression={$_.temperature.ToString("F1")}; Width=10
}, @{
    Label="Humidity (%)"; Expression={$_.humidity.ToString("F0")}; Width=13
}, @{
    Label="Wind (km/h)"; Expression={$_.windSpeed.ToString("F1")}; Width=12
}, @{
    Label="Conditions"; Expression={$_.description}; Width=20
}, @{
    Label="Recorded At"; Expression={$_.recordedAt.ToString("yyyy-MM-dd HH:mm")}; Width=17
} -AutoSize

Write-Host "`n⚠️  ACTIVE WEATHER ALERTS" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Yellow
$alerts = Invoke-RestMethod -Uri "$baseUrl/weatheralerts/active" -Method Get
if ($alerts.Count -gt 0) {
    foreach ($alert in $alerts) {
        Write-Host "`n[$($alert.severity.ToUpper())] $($alert.alertType)" -ForegroundColor $(
            switch ($alert.severity) {
                "Critical" { "Red" }
                "High" { "Red" }
                "Medium" { "Yellow" }
                "Low" { "Green" }
                default { "White" }
            }
        )
        Write-Host "  Description: $($alert.description)"
        Write-Host "  Affected Cities: $($alert.affectedCities -join ', ')"
        Write-Host "  Start: $($alert.startTime.ToString('yyyy-MM-dd HH:mm'))"
        if ($alert.endTime) {
            Write-Host "  End: $($alert.endTime.ToString('yyyy-MM-dd HH:mm'))"
        }
    }
} else {
    Write-Host "No active alerts" -ForegroundColor Green
}

# Display Latest Weather by City
Write-Host "`n`n🌤️  LATEST WEATHER BY CITY" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Yellow
foreach ($city in $cities) {
    $latest = Invoke-RestMethod -Uri "$baseUrl/weatherrecords/city/$($city.id)/latest" -Method Get -ErrorAction SilentlyContinue
    if ($latest) {
        $tempColor = if ($latest.temperature -gt 25) { "Red" } 
                     elseif ($latest.temperature -lt 10) { "Cyan" } 
                     else { "Green" }
        
        Write-Host "`n$($city.name), $($city.country)" -ForegroundColor White -NoNewline
        Write-Host " - " -NoNewline
        Write-Host "$($latest.temperature.ToString('F1'))°C" -ForegroundColor $tempColor
        Write-Host "  Humidity: $($latest.humidity.ToString('F0'))% | Wind: $($latest.windSpeed.ToString('F1')) km/h"
        Write-Host "  Conditions: $($latest.description)"
        Write-Host "  Last Updated: $($latest.recordedAt.ToString('yyyy-MM-dd HH:mm'))" -ForegroundColor Gray
    }
}

# Statistics
Write-Host "`n`n📊 STATISTICS" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Yellow
Write-Host "Total Cities: $($cities.Count)"
Write-Host "Total Weather Records: $($records.Count)"
Write-Host "Active Alerts: $($alerts.Count)"

$avgTemp = ($records | Measure-Object -Property temperature -Average).Average
$maxTemp = ($records | Measure-Object -Property temperature -Maximum).Maximum
$minTemp = ($records | Measure-Object -Property temperature -Minimum).Minimum

Write-Host "`nTemperature Statistics:"
Write-Host "  Average: $($avgTemp.ToString('F1'))°C"
Write-Host "  Highest: $($maxTemp.ToString('F1'))°C"
Write-Host "  Lowest: $($minTemp.ToString('F1'))°C"

Write-Host "`n════════════════════════════════════════════════════════════════`n" -ForegroundColor Cyan
