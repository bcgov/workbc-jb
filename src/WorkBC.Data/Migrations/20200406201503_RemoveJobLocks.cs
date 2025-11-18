using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveJobLocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AdminUsers_LockedByAdminUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_LockedByAdminUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DateLocked",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LockedByAdminUserId",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateLocked",
                table: "Jobs",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockedByAdminUserId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_LockedByAdminUserId",
                table: "Jobs",
                column: "LockedByAdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AdminUsers_LockedByAdminUserId",
                table: "Jobs",
                column: "LockedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
