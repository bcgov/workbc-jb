using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ReCreateExpiredJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpiredJobs",
                columns: table => new
                {
                    JobId = table.Column<long>(nullable: false),
                    RemovedFromElasticsearch = table.Column<bool>(nullable: false),
                    DateRemoved = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiredJobs", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_ExpiredJobs_JobIds_JobId",
                        column: x => x.JobId,
                        principalTable: "JobIds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpiredJobs");
        }
    }
}
