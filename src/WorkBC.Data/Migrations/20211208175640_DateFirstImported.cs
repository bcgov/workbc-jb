using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class DateFirstImported : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalDatePosted",
                table: "JobVersions",
                newName: "DateFirstImported");

            migrationBuilder.Sql(@"
                UPDATE jv
                SET jv.DateFirstImported = DateCreated.DateVersionStart
                FROM JobVersions jv
                INNER JOIN
                (SELECT JobId, Min(DateVersionStart) As DateVersionStart
                FROM JobVersions GROUP BY JobId) DateCreated
                ON DateCreated.JobId = jv.JobId ");

            migrationBuilder.Sql(@"DELETE FROM ReportPersistenceControl WHERE TableName = 'JobStats'");
            migrationBuilder.Sql(@"DELETE FROM JobStats");


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
		   jv.DateFirstImported,
		   jv.PositionsAvailable
	FROM JobVersions jv
		INNER JOIN PeriodVersion pv ON pv.JobId = jv.JobId
			AND pv.VersionNumber = jv.VersionNumber
)
");

			migrationBuilder.Sql(@"
/* Generates data in the JobStats table for a 1-week period.
 *
 * Created by Mike Olund <mike@oxd.com>
 * April 5, 2020
 *
 * NOTE:
 * Stored procedures and functions are updated using code-first migrations.
 * In order to keep localdev, dev, test and prod environments in sync,
 * they should never be modified directly in the sql database (unless
 * you don't mind having your changes wiped out by a future release).
 * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
 */
ALTER PROCEDURE [dbo].[usp_GenerateJobStats]
(
	@WeekEndDate DATETIME
)
AS

BEGIN

DECLARE @StartDate DATETIME;
DECLARE @PeriodId INT;
DECLARE @EndDatePlus1 DATETIME;
DECLARE @TableName varchar(25);

SET @TableName = 'JobStats';

-- Get the WeeklyPeriod record
SELECT @StartDate = WeekStartDate, @PeriodId = Id
FROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;

SET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);

--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record
IF EXISTS (SELECT * FROM ReportPersistenceControl
		   WHERE TableName = @TableName
		   AND WeeklyPeriodId = @PeriodId
		   AND DateCalculated < DateAdd(hour, 48, @WeekEndDate))
BEGIN
	DELETE FROM ReportPersistenceControl
	WHERE TableName = @TableName AND WeeklyPeriodId = @PeriodId;
	-- also delete associated record from JobStats
	DELETE FROM JobStats WHERE WeeklyPeriodId = @PeriodId;
END

BEGIN TRY
	IF NOT EXISTS (SELECT * FROM ReportPersistenceControl
				   WHERE WeeklyPeriodId = @PeriodId
				   AND TableName = @TableName)
	BEGIN
		-- insert a record into ReportPersistenceControl
		INSERT INTO ReportPersistenceControl (WeeklyPeriodId, TableName, DateCalculated, IsTotalToDate)
		SELECT @PeriodId AS WeeklyPeriodId, @TableName AS TableName,
			GetDate() AS DateCalculated,
			(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;

		-- insert records into JobStats
		INSERT INTO JobStats (WeeklyPeriodId, JobSourceId, RegionId, JobPostings, PositionsAvailable)
		SELECT @PeriodId AS WeeklyPeriodId,
			j.JobSourceId,
			ISNULL(l.RegionId, 0) AS RegionId,
			COUNT(*) AS JobPostings,
			SUM(PositionsAvailable) AS PositionsAvailable
		FROM dbo.tvf_GetJobsForDate(@EndDatePlus1) j
			LEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId
		WHERE j.DateFirstImported >= @StartDate AND j.DateFirstImported < @EndDatePlus1
		GROUP BY j.JobSourceId, l.RegionId;
	END

END TRY

BEGIN CATCH
    -- if an error occurs, undo any inserts
	DELETE FROM ReportPersistenceControl
	WHERE TableName = @TableName AND WeeklyPeriodId = @PeriodId;
	-- also delete associated record from JobStats
	DELETE FROM JobStats WHERE WeeklyPeriodId = @PeriodId;

	RETURN -1
END CATCH

RETURN 0

END
");


		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateFirstImported",
                table: "JobVersions",
                newName: "OriginalDatePosted");
        }
    }
}
