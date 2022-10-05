using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddEmailToTvf_GetJobSeekersForDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
	       js.Email,
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
	WHERE @EndDatePlus1 < GETDATE()

	UNION 

		SELECT js.Id AS AspNetUserId, 
		   js.Email,
		   js.LocationId,
		   js.ProvinceId,
		   js.CountryId,
		   js.DateRegistered,
		   js.AccountStatus,
		   js.EmailConfirmed,
		   jsf.IsApprentice,
		   jsf.IsIndigenousPerson,
		   jsf.IsMatureWorker,
		   jsf.IsNewImmigrant,
		   jsf.IsPersonWithDisability,
		   jsf.IsStudent,
		   jsf.IsVeteran,
		   jsf.IsVisibleMinority,
		   jsf.IsYouth
	FROM AspNetUsers js
	LEFT OUTER JOIN JobSeekerFlags jsf ON jsf.AspnetUserId = js.Id
	WHERE @EndDatePlus1 >= GETDATE()
)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
