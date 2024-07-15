using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateJBAdminForNOC2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
/****** Object:  UserDefinedFunction [dbo].[tvf_GetJobsForDate]    Script Date: 7/10/2024 11:52:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* Returns a snapshot of a subset job data for a specified date.
 * Only data the is used by existing reports is included in the snapshot.
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
		   jv.NocCodeId2021,
		   jv.IndustryId,
		   jv.DateFirstImported,
		   jv.PositionsAvailable
	FROM JobVersions jv
		INNER JOIN PeriodVersion pv ON pv.JobId = jv.JobId 
			AND pv.VersionNumber = jv.VersionNumber
)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
