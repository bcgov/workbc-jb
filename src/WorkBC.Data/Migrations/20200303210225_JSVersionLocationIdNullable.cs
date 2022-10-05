using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JSVersionLocationIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerVersions_Locations_LocationId",
                table: "JobSeekerVersions");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "JobSeekerVersions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerVersions_Locations_LocationId",
                table: "JobSeekerVersions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerVersions_Locations_LocationId",
                table: "JobSeekerVersions");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "JobSeekerVersions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerVersions_Locations_LocationId",
                table: "JobSeekerVersions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
