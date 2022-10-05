using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class LockFieldsForAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "AdminUsers");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AdminUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LockBy",
                table: "AdminUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockDate",
                table: "AdminUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "LockBy",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "LockDate",
                table: "AdminUsers");

            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "AdminUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
