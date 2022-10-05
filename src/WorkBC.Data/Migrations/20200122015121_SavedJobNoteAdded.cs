using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SavedJobNoteAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SavedJobs",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NoteUpdatedDate",
                table: "SavedJobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SavedJobs");

            migrationBuilder.DropColumn(
                name: "NoteUpdatedDate",
                table: "SavedJobs");
        }
    }
}
