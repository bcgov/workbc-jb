using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddFieldReIndexNeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReIndexNeeded",
                table: "ImportedJobsWanted",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReIndexNeeded",
                table: "ImportedJobsFederal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReIndexNeeded",
                table: "ImportedJobsWanted");

            migrationBuilder.DropColumn(
                name: "ReIndexNeeded",
                table: "ImportedJobsFederal");
        }
    }
}
