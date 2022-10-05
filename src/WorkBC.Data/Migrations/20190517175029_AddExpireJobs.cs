using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddExpireJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIndexed",
                table: "ImportedJobsFederal");

            migrationBuilder.CreateTable(
                name: "ExpiredJob",
                columns: table => new
                {
                    JobId = table.Column<long>(nullable: false),
                    JobPostEnglish = table.Column<string>(nullable: true),
                    JobPostFrench = table.Column<string>(nullable: true),
                    IsProcessed = table.Column<bool>(nullable: false),
                    JobSource = table.Column<string>(nullable: true),
                    DateRemoved = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiredJob", x => x.JobId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpiredJob");

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexed",
                table: "ImportedJobsFederal",
                nullable: false,
                defaultValue: false);
        }
    }
}
