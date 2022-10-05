using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobsByRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarYear = table.Column<short>(nullable: false),
                    CalendarMonth = table.Column<byte>(nullable: false),
                    FiscalYear = table.Column<short>(nullable: false),
                    WeekOfMonth = table.Column<byte>(nullable: false),
                    WeekStartDate = table.Column<DateTime>(nullable: false),
                    WeekEndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobsByRegion",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    JobSource = table.Column<byte>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    JobPostings = table.Column<int>(nullable: false),
                    PositionsAvailable = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsByRegion", x => new { x.WeeklyPeriodId, x.RegionId, x.JobSource });
                    table.ForeignKey(
                        name: "FK_JobsByRegion_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsByRegion_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobsByRegionPeriods",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    DateCalculated = table.Column<DateTime>(nullable: false),
                    IsTotalToDate = table.Column<bool>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_JobsByRegion_RegionId",
                table: "JobsByRegion",
                column: "RegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsByRegion");

            migrationBuilder.DropTable(
                name: "JobsByRegionPeriods");

            migrationBuilder.DropTable(
                name: "WeeklyPeriods");
        }
    }
}
