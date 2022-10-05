using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MatrixTableRefactor1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsByRegionPeriods");

            migrationBuilder.DropTable(
                name: "JobSeekersByLocationPeriods");

            migrationBuilder.CreateTable(
                name: "JobSeekerAccountReport",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    Label = table.Column<string>(maxLength: 50, nullable: false),
                    Group = table.Column<string>(maxLength: 10, nullable: true),
                    SortOrder = table.Column<short>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerAccountReport", x => new { x.WeeklyPeriodId, x.Label });
                    table.ForeignKey(
                        name: "FK_JobSeekerAccountReport_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportPersistenceControl",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    Report = table.Column<string>(maxLength: 25, nullable: true),
                    DateCalculated = table.Column<DateTime>(nullable: false),
                    IsTotalToDate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPersistenceControl", x => x.WeeklyPeriodId);
                    table.ForeignKey(
                        name: "FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSeekerAccountReport");

            migrationBuilder.DropTable(
                name: "ReportPersistenceControl");

            migrationBuilder.CreateTable(
                name: "JobsByRegionPeriods",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(type: "int", nullable: false),
                    DateCalculated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTotalToDate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsByRegionPeriods", x => x.WeeklyPeriodId);
                    table.ForeignKey(
                        name: "FK_JobsByRegionPeriods_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekersByLocationPeriods",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(type: "int", nullable: false),
                    DateCalculated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTotalToDate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekersByLocationPeriods", x => x.WeeklyPeriodId);
                    table.ForeignKey(
                        name: "FK_JobSeekersByLocationPeriods_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
