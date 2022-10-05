using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobsByRegionRemoveFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobsByRegion_Regions_RegionId",
                table: "JobsByRegion");

            migrationBuilder.DropIndex(
                name: "IX_JobsByRegion_RegionId",
                table: "JobsByRegion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobsByRegion_RegionId",
                table: "JobsByRegion",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobsByRegion_Regions_RegionId",
                table: "JobsByRegion",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
