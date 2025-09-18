using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MatrixTableRefactor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl");

            migrationBuilder.AlterColumn<string>(
                name: "Report",
                table: "ReportPersistenceControl",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl",
                columns: new[] { "WeeklyPeriodId", "Report" });

            migrationBuilder.Sql("ALTER PROCEDURE [dbo].[usp_PopulateJobSeekersByLocation] \r\n(\r\n\t@WeekEndDate DateTime\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DateTime;\r\nDECLARE @PeriodId Int;\r\nDECLARE @ReportName varchar(25);\r\n\r\nSET @ReportName = 'JobSeekersByLocation';\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\t--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t   WHERE Report = @ReportName \r\n\t\t\t   AND WeeklyPeriodId = @PeriodId \r\n\t\t\t   AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM ReportPersistenceControl \r\n\t\tWHERE Report = @ReportName AND WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobSeekersByLocation\r\n\t\tDELETE FROM JobSeekersByLocation WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t\t   WHERE WeeklyPeriodId = @PeriodId \r\n\t\t\t\t   AND Report = @ReportName)\r\n\tBEGIN \r\n\t\t-- insert a record into ReportPersistenceControl\r\n\t\tINSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, @ReportName AS Report, \r\n\t\t\tGetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert BC Region records into JobSeekersByLocation\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tIsNull(l.RegionId, 0) AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.DateRegistered >= @StartDate AND j.DateRegistered < @WeekEndDate \r\n\t\tGROUP BY l.RegionId;\r\n\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\t-1 AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) \r\n\t\tWHERE CountryId = 37 AND ProvinceId <> 2 \r\n\t\t\tAND DateRegistered >= @StartDate AND DateRegistered < @WeekEndDate \r\n\r\n\t\tINSERT INTO JobSeekersByLocation (WeeklyPeriodId, RegionId, Users)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\t-2 AS RegionId,\r\n\t\t\tCount(*) AS Users\r\n\t\tFROM dbo.tvf_GetJobSeekersForDate(@WeekEndDate) \r\n\t\tWHERE (CountryId IS NOT NULL AND CountryId <> 37) \r\n\t\t\tAND (ProvinceId IS NULL OR ProvinceId <> 2)\r\n\t\t\tAND DateRegistered >= @StartDate AND DateRegistered < @WeekEndDate \r\n\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\n\tRETURN -1\r\nEND CATCH\r\n\r\nRETURN 0\r\n\r\nEND");

            migrationBuilder.Sql("ALTER PROCEDURE [dbo].[usp_PopulateJobsByRegion] \r\n(\r\n\t@WeekEndDate DateTime\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DateTime;\r\nDECLARE @PeriodId Int;\r\nDECLARE @ReportName varchar(25);\r\n\r\nSET @ReportName = 'JobsByRegionOrSource';\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\t--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t   WHERE Report = @ReportName \r\n\t\t\t   AND WeeklyPeriodId = @PeriodId \r\n\t\t\t   AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM ReportPersistenceControl \r\n\t\tWHERE Report = @ReportName AND WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobsByRegion\r\n\t\tDELETE FROM JobsByRegion WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t\t   WHERE WeeklyPeriodId = @PeriodId \r\n\t\t\t\t   AND Report = @ReportName) \r\n\tBEGIN \r\n\t\t-- insert a record into ReportPersistenceControl\r\n\t\tINSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, @ReportName AS Report, \r\n\t\t\tGetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert records into JobsByRegion\r\n\t\tINSERT INTO JobsByRegion (WeeklyPeriodId, JobSourceId, RegionId, JobPostings, PositionsAvailable)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tj.JobSourceId, \r\n\t\t\tIsNull(l.RegionId, 0) AS RegionId,\r\n\t\t\tCount(*) AS JobPostings,\r\n\t\t\tSum(PositionsAvailable) AS PositionsAvailable\r\n\t\tFROM dbo.tvf_GetJobsForDate(@WeekEndDate) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.OriginalDatePosted >= @StartDate AND j.OriginalDatePosted < @WeekEndDate \r\n\t\tGROUP BY j.JobSourceId, l.RegionId;\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\n\tRETURN -1\r\nEND CATCH\r\n\r\nRETURN 0\r\n\r\nEND\r\n");

            migrationBuilder.Sql("DELETE FROM [dbo].[JobSeekersByLocation]");

            migrationBuilder.Sql("DELETE FROM [dbo].[JobsByRegion]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl");

            migrationBuilder.AlterColumn<string>(
                name: "Report",
                table: "ReportPersistenceControl",
                type: "varchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl",
                column: "WeeklyPeriodId");
        }
    }
}
