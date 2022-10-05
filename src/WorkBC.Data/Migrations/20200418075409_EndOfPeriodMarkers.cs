using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class EndOfPeriodMarkers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEndOfFiscalYear",
                table: "WeeklyPeriods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEndOfMonth",
                table: "WeeklyPeriods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("DELETE FROM JobSeekersByLocation");
            migrationBuilder.Sql("DELETE FROM JobsByRegion");
            migrationBuilder.Sql("DELETE FROM JobSeekerStats");
            migrationBuilder.Sql("DELETE FROM ReportPersistenceControl");
            migrationBuilder.Sql("DELETE FROM WeeklyPeriods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEndOfFiscalYear",
                table: "WeeklyPeriods");

            migrationBuilder.DropColumn(
                name: "IsEndOfMonth",
                table: "WeeklyPeriods");
        }
    }
}
