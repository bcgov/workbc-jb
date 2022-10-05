using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AlterGenerationProcedures1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl");

            migrationBuilder.DropColumn(
                name: "Report",
                table: "ReportPersistenceControl");

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "ReportPersistenceControl",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl",
                columns: new[] { "WeeklyPeriodId", "TableName" });

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
DECLARE @TableName NVARCHAR(25);

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
		WHERE j.OriginalDatePosted >= @StartDate AND j.OriginalDatePosted < @EndDatePlus1
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
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl");

            migrationBuilder.DropColumn(
                name: "TableName",
                table: "ReportPersistenceControl");

            migrationBuilder.AddColumn<string>(
                name: "Report",
                table: "ReportPersistenceControl",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportPersistenceControl",
                table: "ReportPersistenceControl",
                columns: new[] { "WeeklyPeriodId", "Report" });
        }
    }
}
