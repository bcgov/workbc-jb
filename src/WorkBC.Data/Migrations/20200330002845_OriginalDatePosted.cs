using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class OriginalDatePosted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalDatePosted",
                table: "JobVersions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("Update Jobs SET ActualDatePosted = DatePosted");
            migrationBuilder.Sql("Update JobVersions Set ActualDatePosted = DatePosted");
            migrationBuilder.Sql("Update JobVersions Set OriginalDatePosted = (Select TOP 1 DatePosted From JobVersions jv2 Where jv2.JobId = JobVersions.JobId AND Jv2.VersionNumber = 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalDatePosted",
                table: "JobVersions");
        }
    }
}
