using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RegionFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobStats_RegionId",
                table: "JobStats",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerStats_RegionId",
                table: "JobSeekerStats",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerStats_Regions_RegionId",
                table: "JobSeekerStats",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobStats_Regions_RegionId",
                table: "JobStats",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(
                "INSERT INTO [dbo].[Regions] ([Id],[Name],[ListOrder],[IsHidden]) VALUES (0, 'Location Not Available',1003,1)");

            migrationBuilder.Sql("DROP PROCEDURE [usp_GenerateJobsStats]");
            migrationBuilder.Sql("\r\n/* Generates data in the JobStats table for a 1-week period.\r\n *\r\n * Created by Mike Olund <mike@oxd.com>\r\n * March 15, 2020\r\n *\r\n * NOTE:\r\n * Stored procedures and functions are updated using code-first migrations. \r\n * In order to keep localdev, dev, test and prod environments in sync,\r\n * they should never be modified directly in the sql database (unless \r\n * you don't mind having your changes wiped out by a future release).\r\n * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!\r\n */\r\nCREATE PROCEDURE [dbo].[usp_GenerateJobStats] \r\n(\r\n\t@WeekEndDate DATETIME,\r\n\t@StoreName NVARCHAR(25)\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DATETIME;\r\nDECLARE @PeriodId INT;\r\nDECLARE @EndDatePlus1 DATETIME;\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\tSET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);\r\n\r\n\t--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t   WHERE Report = @StoreName \r\n\t\t\t   AND WeeklyPeriodId = @PeriodId \r\n\t\t\t   AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM ReportPersistenceControl \r\n\t\tWHERE Report = @StoreName AND WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobStats\r\n\t\tDELETE FROM JobStats WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t\t   WHERE WeeklyPeriodId = @PeriodId \r\n\t\t\t\t   AND Report = @StoreName) \r\n\tBEGIN \r\n\t\t-- insert a record into ReportPersistenceControl\r\n\t\tINSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, @StoreName AS Report, \r\n\t\t\tGetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert records into JobStats\r\n\t\tINSERT INTO JobStats (WeeklyPeriodId, JobSourceId, RegionId, JobPostings, PositionsAvailable)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tj.JobSourceId, \r\n\t\t\tISNULL(l.RegionId, 0) AS RegionId,\r\n\t\t\tCOUNT(*) AS JobPostings,\r\n\t\t\tSUM(PositionsAvailable) AS PositionsAvailable\r\n\t\tFROM dbo.tvf_GetJobsForDate(@EndDatePlus1) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.OriginalDatePosted >= @StartDate AND j.OriginalDatePosted < @EndDatePlus1\r\n\t\tGROUP BY j.JobSourceId, l.RegionId;\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\n\tRETURN -1\r\nEND CATCH\r\n\r\nRETURN 0\r\n\r\nEND\r\n");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerStats_Regions_RegionId",
                table: "JobSeekerStats");

            migrationBuilder.DropForeignKey(
                name: "FK_JobStats_Regions_RegionId",
                table: "JobStats");

            migrationBuilder.DropIndex(
                name: "IX_JobStats_RegionId",
                table: "JobStats");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerStats_RegionId",
                table: "JobSeekerStats");

            migrationBuilder.Sql("DELETE FROM Regions WHERE ID = 0");
        }
    }
}
