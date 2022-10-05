using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddPurgedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisplayUntil",
                table: "ImportedJobsFederal",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexed",
                table: "ImportedJobsFederal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayUntil",
                table: "ImportedJobsFederal");

            migrationBuilder.DropColumn(
                name: "IsIndexed",
                table: "ImportedJobsFederal");
        }
    }
}
