using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class VersionsUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobVersions_JobId",
                table: "JobVersions");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerVersions_AspNetUserId",
                table: "JobSeekerVersions");

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_JobId_VersionNumber",
                table: "JobVersions",
                columns: new[] { "JobId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_AspNetUserId_VersionNumber",
                table: "JobSeekerVersions",
                columns: new[] { "AspNetUserId", "VersionNumber" },
                unique: true,
                filter: "[AspNetUserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobVersions_JobId_VersionNumber",
                table: "JobVersions");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerVersions_AspNetUserId_VersionNumber",
                table: "JobSeekerVersions");

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_JobId",
                table: "JobVersions",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_AspNetUserId",
                table: "JobSeekerVersions",
                column: "AspNetUserId");
        }
    }
}
