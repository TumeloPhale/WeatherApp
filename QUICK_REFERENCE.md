# Weather App - Quick Reference

## 🚀 Starting the Application

### Option 1: Using the PowerShell Script
```powershell
cd c:\dev\WeatherApp
.\Start-WeatherApp.ps1
```

### Option 2: Manual Start
```powershell
cd c:\dev\WeatherApp\WeatherApp.API
dotnet run
```

### Access Points
- **API Documentation (Scalar UI)**: http://localhost:5030/scalar/v1
- **Health Check**: http://localhost:5030/health
- **OpenAPI Spec**: http://localhost:5030/openapi/v1.json

---

## 📊 Database Schema

### Tables
1. **Cities** - Stores city information
2. **WeatherRecords** - Weather data for cities (One-to-Many with Cities)
3. **WeatherAlerts** - Weather alerts
4. **CityWeatherAlert** - Junction table (Many-to-Many between Cities and Alerts)
5. **VersionInfo** - FluentMigrator version tracking

### Relationships
```
City (1) ──── (Many) WeatherRecord
City (Many) ──── (Many) WeatherAlert (via CityWeatherAlert)
```

---

## 🔌 API Endpoints Quick Reference

### Cities
```
GET    /api/cities                       # Get all cities
GET    /api/cities/{id}                  # Get city by ID
POST   /api/cities                       # Create city
GET    /api/cities/with-active-alerts    # Cities with alerts
```

### Weather Records
```
GET    /api/weatherrecords               # Get all records
GET    /api/weatherrecords/{id}          # Get record by ID
GET    /api/weatherrecords/city/{cityId} # Records for city
GET    /api/weatherrecords/city/{cityId}/latest  # Latest for city
POST   /api/weatherrecords               # Create record
```

### Weather Alerts
```
GET    /api/weatheralerts                # Get all alerts
GET    /api/weatheralerts/active         # Active alerts only
GET    /api/weatheralerts/city/{cityId}  # Alerts for city
POST   /api/weatheralerts                # Create alert
PUT    /api/weatheralerts/{id}/deactivate  # Deactivate
```

---

## 📝 Sample API Calls

### Get All Cities
```bash
curl http://localhost:5030/api/cities
```

### Create a New City
```bash
curl -X POST http://localhost:5030/api/cities \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Berlin",
    "country": "Germany",
    "latitude": 52.5200,
    "longitude": 13.4050
  }'
```

### Create Weather Record
```bash
curl -X POST http://localhost:5030/api/weatherrecords \
  -H "Content-Type: application/json" \
  -d '{
    "cityId": 1,
    "temperature": 18.5,
    "humidity": 70,
    "windSpeed": 15,
    "description": "Cloudy"
  }'
```

### Get Latest Weather for City
```bash
curl http://localhost:5030/api/weatherrecords/city/1/latest
```

### Create Weather Alert
```bash
curl -X POST http://localhost:5030/api/weatheralerts \
  -H "Content-Type: application/json" \
  -d '{
    "alertType": "Storm",
    "severity": "High",
    "description": "Severe weather warning",
    "startTime": "2025-11-11T15:00:00Z",
    "endTime": "2025-11-11T21:00:00Z",
    "cityIds": [1, 2]
  }'
```

### Get Active Alerts
```bash
curl http://localhost:5030/api/weatheralerts/active
```

---

## ✅ Validation Rules

### City
- ✓ Name and Country required
- ✓ No duplicates (unique Name + Country)
- ✓ Latitude: -90 to 90
- ✓ Longitude: -180 to 180

### Weather Record
- ✓ City must exist
- ✓ Temperature: -100 to 100°C
- ✓ Humidity: 0 to 100%
- ✓ Wind Speed: 0 to 500 km/h

### Weather Alert
- ✓ Alert Type: Storm, Heatwave, Flood, Snow, Fog, Wind, Tornado, Hurricane
- ✓ Severity: Low, Medium, High, Critical
- ✓ Description: min 10 characters
- ✓ EndTime must be after StartTime
- ✓ All cities in cityIds must exist

---

## 🗂️ Project Structure

```
WeatherApp/
├── WeatherApp.API/          # Controllers, Program.cs
├── WeatherApp.Services/     # Business logic, validation
├── WeatherApp.Data/         # DbContext, repositories
├── WeatherApp.Domain/       # Entities, interfaces
├── WeatherApp.Migrations/   # FluentMigrator migrations
├── TESTING_GUIDE.md         # Detailed testing guide
├── PROJECT_SUMMARY.md       # Complete project summary
└── Start-WeatherApp.ps1     # Quick start script
```

---

## 🎯 Key Features

✅ **Layered Architecture** - Clean separation of concerns
✅ **Entity Framework Core** - ORM with navigation properties
✅ **FluentMigrator** - Version-controlled migrations
✅ **Async/Await** - Async operations throughout
✅ **Validation** - Business logic in service layer
✅ **Dependency Injection** - All layers use DI
✅ **Logging** - Structured logging with ILogger
✅ **Health Checks** - Monitor application health
✅ **API Documentation** - Interactive Scalar UI

---

## 🛠️ Troubleshooting

### Database Issues
If you encounter database errors:
1. Ensure SQL Server LocalDB is running
2. Delete the database: `sqllocaldb delete mssqllocaldb`
3. Restart the app (it will recreate everything)

### Build Issues
```powershell
cd c:\dev\WeatherApp
dotnet clean
dotnet restore
dotnet build
```

### Port Already in Use
If port 5030 is busy, edit `WeatherApp.API/Properties/launchSettings.json` to change the port.

---

## 📚 Files to Review

1. **Entities**: `WeatherApp.Domain/Entities/*.cs`
2. **Migrations**: `WeatherApp.Migrations/*.cs`
3. **Services**: `WeatherApp.Services/*.cs`
4. **Repositories**: `WeatherApp.Data/Repositories/*.cs`
5. **Controllers**: `WeatherApp.API/Controllers/*.cs`
6. **DbContext**: `WeatherApp.Data/WeatherDbContext.cs`
7. **Startup**: `WeatherApp.API/Program.cs`

---

## 📊 Seeded Data

### Cities (5)
1. London, United Kingdom
2. New York, United States
3. Tokyo, Japan
4. Sydney, Australia
5. Paris, France

### Weather Records (5)
- London: 2 records
- New York: 2 records
- Tokyo: 1 record

### Weather Alerts (2)
1. Storm (affecting London & Paris)
2. Heatwave (affecting Sydney)

---

**Need more details? See TESTING_GUIDE.md or PROJECT_SUMMARY.md**
