using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobRefactoring4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "OriginalDatePosted",
                "Jobs");

            migrationBuilder.RenameColumn(
                "LastUpdated",
                "ImportedJobsWanted",
                "DateLastImported");

            migrationBuilder.RenameColumn(
                "OriginDateStamp",
                "ImportedJobsWanted",
                "ApiDate");

            migrationBuilder.RenameColumn(
                "LastUpdated",
                "ImportedJobsFederal",
                "DateLastImported");

            migrationBuilder.RenameColumn(
                "OriginDateStamp",
                "ImportedJobsFederal",
                "ApiDate");

            migrationBuilder.RenameColumn(
                "LastUpdated",
                "ExpiredJobs",
                "DateLastImported");

            migrationBuilder.RenameColumn(
                "OriginDateStamp",
                "ExpiredJobs",
                "ApiDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}