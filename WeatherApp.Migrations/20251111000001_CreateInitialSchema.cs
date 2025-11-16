using FluentMigrator;

namespace WeatherApp.Migrations;

[Migration(20251111000001)]
public class CreateInitialSchema : Migration
{
    public override void Up()
    {
        // Create Cities table
        Create.Table("Cities")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Country").AsString(100).NotNullable()
            .WithColumn("Latitude").AsDecimal(9, 6).NotNullable()
            .WithColumn("Longitude").AsDecimal(9, 6).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedAt").AsDateTime().Nullable();

        // Create unique index on Name + Country
        Create.Index("IX_Cities_Name_Country")
            .OnTable("Cities")
            .OnColumn("Name").Ascending()
            .OnColumn("Country").Ascending()
            .WithOptions().Unique();

        // Create WeatherRecords table
        Create.Table("WeatherRecords")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("CityId").AsInt32().NotNullable()
            .WithColumn("Temperature").AsDecimal(5, 2).NotNullable()
            .WithColumn("Humidity").AsDecimal(5, 2).NotNullable()
            .WithColumn("WindSpeed").AsDecimal(5, 2).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn("RecordedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        // Create foreign key
        Create.ForeignKey("FK_WeatherRecords_Cities_CityId")
            .FromTable("WeatherRecords").ForeignColumn("CityId")
            .ToTable("Cities").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        // Create index for efficient queries
        Create.Index("IX_WeatherRecords_CityId_RecordedAt")
            .OnTable("WeatherRecords")
            .OnColumn("CityId").Ascending()
            .OnColumn("RecordedAt").Descending();

        // Create WeatherAlerts table
        Create.Table("WeatherAlerts")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("AlertType").AsString(50).NotNullable()
            .WithColumn("Severity").AsString(20).NotNullable()
            .WithColumn("Description").AsString(1000).NotNullable()
            .WithColumn("StartTime").AsDateTime().NotNullable()
            .WithColumn("EndTime").AsDateTime().Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        // Create junction table for many-to-many relationship
        Create.Table("CityWeatherAlert")
            .WithColumn("CityId").AsInt32().NotNullable()
            .WithColumn("WeatherAlertId").AsInt32().NotNullable();

        // Create composite primary key
        Create.PrimaryKey("PK_CityWeatherAlert")
            .OnTable("CityWeatherAlert")
            .Columns("CityId", "WeatherAlertId");

        // Create foreign keys for junction table
        Create.ForeignKey("FK_CityWeatherAlert_Cities_CityId")
            .FromTable("CityWeatherAlert").ForeignColumn("CityId")
            .ToTable("Cities").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_CityWeatherAlert_WeatherAlerts_WeatherAlertId")
            .FromTable("CityWeatherAlert").ForeignColumn("WeatherAlertId")
            .ToTable("WeatherAlerts").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("CityWeatherAlert");
        Delete.Table("WeatherRecords");
        Delete.Table("WeatherAlerts");
        Delete.Table("Cities");
    }
}
