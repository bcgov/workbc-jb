using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RefactorJobsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFirstSeen",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Noc",
                table: "Jobs",
                "Noc2016");

            migrationBuilder.RenameColumn(
                name: "DateImported",
                table: "ImportedJobsWanted", 
                "DateFirstImported");

            migrationBuilder.RenameColumn(
                name: "DateRefreshed",
                table: "ImportedJobsWanted",
                "OriginDateStamp");

            migrationBuilder.RenameColumn(
                name: "JobXml",
                table: "ImportedJobsWanted", 
                "JobPostEnglish");

            migrationBuilder.RenameColumn(
                name: "DateImported",
                table: "ImportedJobsFederal",
                "DateFirstImported");

            migrationBuilder.RenameColumn(
                name: "FileUpdateDate",
                table: "ImportedJobsFederal",
                "OriginDateStamp");

            migrationBuilder.RenameColumn(
                name: "IsProcessed",
                table: "ExpiredJobs", 
                "RemovedFromIndex");

            migrationBuilder.DropColumn(
                name: "JobSource",
                table: "ExpiredJobs");

            migrationBuilder.AddColumn<byte>(
                name: "JobSource",
                table: "ExpiredJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFirstImported",
                table: "ExpiredJobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginDateStamp",
                table: "ExpiredJobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
