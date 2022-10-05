using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class usp_PopulateJobsByRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE PROCEDURE [dbo].[usp_PopulateJobsByRegion] (@WeekEndDate DateTime)\r\n\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DateTime;\r\nDECLARE @PeriodId Int;\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\t--- Check if a JobsByRegionPeriods record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM JobsByRegionPeriods WHERE WeeklyPeriodId = @PeriodId AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM JobsByRegionPeriods WHERE WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobsByRegion\r\n\t\tDELETE FROM JobsByRegion WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM JobsByRegionPeriods WHERE WeeklyPeriodId = @PeriodId) \r\n\tBEGIN \r\n\t\t-- insert a record into JobsByRegionPeriods\r\n\t\tINSERT INTO JobsByRegionPeriods (WeeklyPeriodId, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, GetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert records into JobsByRegion\r\n\t\tINSERT INTO JobsByRegion (WeeklyPeriodId, JobSource, RegionId, JobPostings, PositionsAvailable)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tj.JobSource, \r\n\t\t\tIsNull(l.RegionId, 0) AS RegionId,\r\n\t\t\tCount(*) AS JobPostings,\r\n\t\t\tSum(PositionsAvailable) AS PositionsAvailable\r\n\t\tFROM dbo.tvf_GetJobsForDate(@WeekEndDate) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.OriginalDatePosted >= @StartDate AND j.OriginalDatePosted < @WeekEndDate \r\n\t\tGROUP BY j.JobSource, l.RegionId;\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\nEND CATCH\r\n\r\nEND\r\n");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[usp_PopulateJobsByRegion]");
        }
    }
}
