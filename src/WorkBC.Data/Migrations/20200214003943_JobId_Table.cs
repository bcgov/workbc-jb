using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobId_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobIds",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateFirstSeen = table.Column<DateTime>(nullable: false),
                    JobSource = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobIds", x => x.Id);
                });

            migrationBuilder.Sql("insert into JobIds select JobId as Id, DateFirstImported as DateFirstSeen, Source As JobSource from Jobs");
            migrationBuilder.Sql("insert into JobIds Select JobId as Id, DateFirstImported as DateFirstSeen, JobSource  From ExpiredJobs Where JobId NOT IN (SELECT Id From JobIds)");
            migrationBuilder.Sql("insert into JobIds Select JobId as Id, DateFirstImported as DateFirstSeen, 1 AS JobSource From ImportedJobsFederal Where JobId NOT IN (SELECT Id From JobIds)");
            migrationBuilder.Sql("insert into JobIds Select JobId as Id, DateFirstImported as DateFirstSeen, 2 AS JobSource From ImportedJobsWanted Where JobId NOT IN (SELECT Id From JobIds)");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpiredJobs_JobIds_JobId",
                table: "ExpiredJobs",
                column: "JobId",
                principalTable: "JobIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportedJobsFederal_JobIds_JobId",
                table: "ImportedJobsFederal",
                column: "JobId",
                principalTable: "JobIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportedJobsWanted_JobIds_JobId",
                table: "ImportedJobsWanted",
                column: "JobId",
                principalTable: "JobIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobIds_JobId",
                table: "Jobs",
                column: "JobId",
                principalTable: "JobIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpiredJobs_JobIds_JobId",
                table: "ExpiredJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportedJobsFederal_JobIds_JobId",
                table: "ImportedJobsFederal");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportedJobsWanted_JobIds_JobId",
                table: "ImportedJobsWanted");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobIds_JobId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobIds");
        }
    }
}
