using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobAlertColumnNameUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElasticSearchQuery",
                table: "JobAlerts");

            migrationBuilder.DropColumn(
                name: "ElasticSearchQueryVersion",
                table: "JobAlerts");

            migrationBuilder.AddColumn<string>(
                name: "JobSearchFilters",
                table: "JobAlerts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobSearchFiltersVersion",
                table: "JobAlerts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobSearchFilters",
                table: "JobAlerts");

            migrationBuilder.DropColumn(
                name: "JobSearchFiltersVersion",
                table: "JobAlerts");

            migrationBuilder.AddColumn<string>(
                name: "ElasticSearchQuery",
                table: "JobAlerts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ElasticSearchQueryVersion",
                table: "JobAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
