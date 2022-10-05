using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveReservist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReservist",
                table: "JobSeekerVersions");

            migrationBuilder.DropColumn(
                name: "IsReservist",
                table: "JobSeekerFlags");

            migrationBuilder.Sql("ALTER FUNCTION [dbo].[tvf_GetJobSeekersForDate](\r\n\t@EndDate DateTime\r\n) \r\nRETURNS TABLE\r\nAS\r\nRETURN\r\n(\r\n\tWITH PeriodVersion(AspNetUserId, VersionNumber) AS \r\n    (\r\n        SELECT AspnetUserId, Max(VersionNumber) AS VersionNumber\r\n        FROM JobSeekerVersions \r\n        WHERE DateVersionStart < @EndDate \r\n            AND (DateVersionEnd IS NULL OR DateVersionEnd >= @EndDate)\r\n        GROUP BY AspnetUserId\r\n    )\r\n\tSELECT js.AspNetUserId, \r\n\t\t   js.LocationId,\r\n\t\t   js.ProvinceId,\r\n\t\t   js.CountryId,\r\n\t\t   js.DateRegistered,\r\n\t\t   js.AccountStatus,\r\n\t\t   js.EmailConfirmed,\r\n\t\t   js.IsApprentice,\r\n\t\t   js.IsIndigenousPerson,\r\n\t\t   js.IsMatureWorker,\r\n\t\t   js.IsNewImmigrant,\r\n\t\t   js.IsPersonWithDisability,\r\n\t\t   js.IsStudent,\r\n\t\t   js.IsVeteran,\r\n\t\t   js.IsVisibleMinority,\r\n\t\t   js.IsYouth\r\n\tFROM JobSeekerVersions js\r\n\t\tINNER JOIN PeriodVersion pv ON pv.AspNetUserId = js.AspNetUserId\r\n\t\t\tAND pv.VersionNumber = js.VersionNumber\r\n)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReservist",
                table: "JobSeekerVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReservist",
                table: "JobSeekerFlags",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
