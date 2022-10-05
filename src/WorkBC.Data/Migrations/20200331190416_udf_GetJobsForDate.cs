using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class udf_GetJobsForDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FUNCTION dbo.tvf_GetJobsForDate(\r\n\t@EndDate DateTime\r\n) \r\nRETURNS TABLE\r\nAS\r\nRETURN\r\n(\r\n\tWITH PeriodVersion(JobId, VersionNumber) AS \r\n\t(\r\n\t SELECT JobVersions.JobId, \r\n\t\t\tMax(JobVersions.VersionNumber) AS VersionNumber\r\n\t\tFROM JobVersions \r\n\t\tWHERE JobVersions.DateVersionStart < @EndDate \r\n\t\t\tAND (JobVersions.DateVersionEnd IS NULL OR JobVersions.DateVersionEnd >= @EndDate)\r\n\t\tGROUP BY JobVersions.JobId \r\n\t)\r\n\tSELECT jv.JobId, \r\n\t\t   jv.JobSource,\r\n\t\t   jv.LocationId, \r\n\t\t   jv.NocCodeId, \r\n\t\t   jv.IndustryId, \r\n\t\t   jv.OriginalDatePosted,\r\n\t\t   jv.PositionsAvailable \r\n\tFROM JobVersions jv \r\n\t\tINNER JOIN PeriodVersion pv ON pv.JobId = jv.JobId \r\n\t\t\tAND pv.VersionNumber = jv.VersionNumber\r\n)");

            migrationBuilder.Sql("CREATE FUNCTION [dbo].[tvf_GetJobSeekersForDate](\r\n\t@EndDate DateTime\r\n) \r\nRETURNS TABLE\r\nAS\r\nRETURN\r\n(\r\n\tWITH PeriodVersion(AspNetUserId, VersionNumber) AS \r\n    (\r\n        SELECT AspnetUserId, Max(VersionNumber) AS VersionNumber\r\n        FROM JobSeekerVersions \r\n        WHERE DateVersionStart < @EndDate \r\n            AND (DateVersionEnd IS NULL OR DateVersionEnd >= @EndDate)\r\n        GROUP BY AspnetUserId\r\n    )\r\n\tSELECT js.AspNetUserId, \r\n\t\t   js.LocationId,\r\n\t\t   js.ProvinceId,\r\n\t\t   js.CountryId,\r\n\t\t   js.DateRegistered,\r\n\t\t   js.AccountStatus,\r\n\t\t   js.EmailConfirmed,\r\n\t\t   js.IsApprentice,\r\n\t\t   js.IsIndigenousPerson,\r\n\t\t   js.IsMatureWorker,\r\n\t\t   js.IsNewImmigrant,\r\n\t\t   js.IsPersonWithDisability,\r\n\t\t   js.IsReservist,\r\n\t\t   js.IsStudent,\r\n\t\t   js.IsVeteran,\r\n\t\t   js.IsVisibleMinority,\r\n\t\t   js.IsYouth\r\n\tFROM JobSeekerVersions js\r\n\t\tINNER JOIN PeriodVersion pv ON pv.AspNetUserId = js.AspNetUserId\r\n\t\t\tAND pv.VersionNumber = js.VersionNumber\r\n)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION [dbo].[tvf_GetJobsForDate]");

            migrationBuilder.Sql("DROP FUNCTION [dbo].[tvf_GetJobSeekersForDate]");
        }
    }
}
