using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdatePopulateJobSeekersByLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER PROCEDURE [dbo].[usp_PopulateJobSeekersByLocation] \r\n(\r\n\t@WeekEndDate DateTime\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DateTime;\r\nDECLARE @PeriodId Int;\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\t--- Check if a JobSeekersByLocationPeriods record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM JobSeekersByLocationPeriods WHERE WeeklyPeriodId = @PeriodId AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM JobSeekersByLocationPeriods WHERE WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobSeekersByLocation\r\n\t\tDELETE FROM JobSeekersByLocation WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM JobSeekersByLocationPeriods WHERE WeeklyPeriodId = @PeriodId) \r\n\tBEGIN \r\n\t\t-- insert a record into JobSeekersByLocationPeriods\r\n\t\tINSERT INTO JobSeekersByLocationPeriods (WeeklyPeriodId, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, GetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert BC Region records into JobSeekersByLocation\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tIsNull(l.RegionId, 0) AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.DateRegistered >= @StartDate AND j.DateRegistered < @WeekEndDate \r\n\t\tGROUP BY l.RegionId;\r\n\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\t-1 AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) \r\n\t\tWHERE CountryId = 37 AND ProvinceId <> 2 \r\n\t\t\tAND DateRegistered >= @StartDate AND DateRegistered < @WeekEndDate \r\n\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\t-2 AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) \r\n\t\tWHERE (CountryId IS NOT NULL AND CountryId <> 37) \r\n\t\t\tAND (ProvinceId IS NULL OR ProvinceId <> 2)\r\n\t\t\tAND DateRegistered >= @StartDate AND DateRegistered < @WeekEndDate \r\n\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\nEND CATCH\r\n\r\nEND");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
