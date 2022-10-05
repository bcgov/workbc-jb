using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class DropExpiredJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpiredJobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpiredJobs",
                columns: table => new
                {
                    JobId = table.Column<long>(type: "bigint", nullable: false),
                    ApiDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLastImported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRemoved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JobPostEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobSourceId = table.Column<byte>(type: "tinyint", nullable: false),
                    RemovedFromElasticsearch = table.Column<bool>(type: "bit", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ExpiredJobs_JobSources_JobSourceId",
                        column: x => x.JobSourceId,
                        principalTable: "JobSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpiredJobs_JobSourceId",
                table: "ExpiredJobs",
                column: "JobSourceId");
        }
    }
}
