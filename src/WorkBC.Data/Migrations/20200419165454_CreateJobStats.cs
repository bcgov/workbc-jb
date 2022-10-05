using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CreateJobStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsByRegion");

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "JobSeekerStats",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobStats",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    JobSourceId = table.Column<byte>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    JobPostings = table.Column<int>(nullable: false),
                    PositionsAvailable = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStats", x => new { x.WeeklyPeriodId, x.RegionId, x.JobSourceId });
                    table.ForeignKey(
                        name: "FK_JobStats_JobSources_JobSourceId",
                        column: x => x.JobSourceId,
                        principalTable: "JobSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobStats_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobStats_JobSourceId",
                table: "JobStats",
                column: "JobSourceId");

            migrationBuilder.Sql(@"
/* Returns a snapshot of a subset jobseeker data for a specified date.
 * Only data the is used by existing reports is included in the snapshot.
 *
 * Created by Mike Olund <mike@oxd.com>
 * March 31, 2020
 *
 * NOTE:
 * Stored procedures and functions are updated using code-first migrations. 
 * In order to keep localdev, dev, test and prod environments in sync,
 * they should never be modified directly in the sql database (unless 
 * you don't mind having your changes wiped out by a future release).
 * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
 */
ALTER FUNCTION [dbo].[tvf_GetJobSeekersForDate](
	@EndDatePlus1 DateTime
) 
RETURNS TABLE
AS
RETURN
(
	WITH PeriodVersion(AspNetUserId, VersionNumber) AS 
    (
        SELECT AspnetUserId, Max(VersionNumber) AS VersionNumber
        FROM JobSeekerVersions 
        WHERE DateVersionStart < @EndDatePlus1
            AND (DateVersionEnd IS NULL OR DateVersionEnd >= @EndDatePlus1)
        GROUP BY AspnetUserId
    )
	SELECT js.AspNetUserId, 
		   js.LocationId,
		   js.ProvinceId,
		   js.CountryId,
		   js.DateRegistered,
		   js.AccountStatus,
		   js.EmailConfirmed,
		   js.IsApprentice,
		   js.IsIndigenousPerson,
		   js.IsMatureWorker,
		   js.IsNewImmigrant,
		   js.IsPersonWithDisability,
		   js.IsStudent,
		   js.IsVeteran,
		   js.IsVisibleMinority,
		   js.IsYouth
	FROM JobSeekerVersions js
		INNER JOIN PeriodVersion pv ON pv.AspNetUserId = js.AspNetUserId
			AND pv.VersionNumber = js.VersionNumber
)
");

            migrationBuilder.Sql(@"
/* Returns a snapshot of a subset job data for a specified date.
 * Only data the is used by existing reports is included in the snapshot.
 *
 * Created by Mike Olund <mike@oxd.com>
 * March 15, 2020
 *
 * NOTE:
 * Stored procedures and functions are updated using code-first migrations. 
 * In order to keep localdev, dev, test and prod environments in sync,
 * they should never be modified directly in the sql database (unless 
 * you don't mind having your changes wiped out by a future release).
 * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
 */
ALTER FUNCTION [dbo].[tvf_GetJobsForDate](
	@EndDatePlus1 DateTime
) 
RETURNS TABLE
AS
RETURN
(
	WITH PeriodVersion(JobId, VersionNumber) AS
	(
	 SELECT JobVersions.JobId,
			Max(JobVersions.VersionNumber) AS VersionNumber
		FROM JobVersions
		WHERE JobVersions.DateVersionStart < @EndDatePlus1
			AND (JobVersions.DateVersionEnd IS NULL OR JobVersions.DateVersionEnd >= @EndDatePlus1)
		GROUP BY JobVersions.JobId 
	)
	SELECT jv.JobId,
		   jv.JobSourceId,
		   jv.LocationId,
		   jv.NocCodeId,
		   jv.IndustryId,
		   jv.OriginalDatePosted,
		   jv.PositionsAvailable
	FROM JobVersions jv
		INNER JOIN PeriodVersion pv ON pv.JobId = jv.JobId 
			AND pv.VersionNumber = jv.VersionNumber
)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobStats");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "JobSeekerStats");

            migrationBuilder.CreateTable(
                name: "JobsByRegion",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    JobSourceId = table.Column<byte>(type: "tinyint", nullable: false),
                    JobPostings = table.Column<int>(type: "int", nullable: false),
                    PositionsAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsByRegion", x => new { x.WeeklyPeriodId, x.RegionId, x.JobSourceId });
                    table.ForeignKey(
                        name: "FK_JobsByRegion_JobSources_JobSourceId",
                        column: x => x.JobSourceId,
                        principalTable: "JobSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsByRegion_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobsByRegion_JobSourceId",
                table: "JobsByRegion",
                column: "JobSourceId");
        }
    }
}
