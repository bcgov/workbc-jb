using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerFlags_AspNetUsers_JobSeekerId",
                table: "JobSeekerFlags");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerFlags_JobSeekerId",
                table: "JobSeekerFlags");

            migrationBuilder.RenameColumn(
                name: "JobSeekerId",
                table: "JobSeekerFlags",
                newName: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerFlags_AspNetUserId",
                table: "JobSeekerFlags",
                column: "AspNetUserId",
                unique: true,
                filter: "[AspNetUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerFlags_AspNetUsers_AspNetUserId",
                table: "JobSeekerFlags",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerFlags_AspNetUsers_AspNetUserId",
                table: "JobSeekerFlags");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerFlags_AspNetUserId",
                table: "JobSeekerFlags");

            migrationBuilder.RenameColumn(
                name: "AspNetUserId",
                table: "JobSeekerFlags",
                newName: "JobSeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerFlags_JobSeekerId",
                table: "JobSeekerFlags",
                column: "JobSeekerId",
                unique: true,
                filter: "[JobSeekerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerFlags_AspNetUsers_JobSeekerId",
                table: "JobSeekerFlags",
                column: "JobSeekerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
