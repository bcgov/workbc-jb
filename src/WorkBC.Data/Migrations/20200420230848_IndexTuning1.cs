using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndexTuning1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WeeklyPeriods_WeekEndDate",
                table: "WeeklyPeriods",
                column: "WeekEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_SavedIndustryProfiles_DateDeleted",
                table: "SavedIndustryProfiles",
                column: "DateDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SavedIndustryProfiles_DateSaved",
                table: "SavedIndustryProfiles",
                column: "DateSaved");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCareerProfiles_DateDeleted",
                table: "SavedCareerProfiles",
                column: "DateDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCareerProfiles_DateSaved",
                table: "SavedCareerProfiles",
                column: "DateSaved");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerEventLog_DateLogged",
                table: "JobSeekerEventLog",
                column: "DateLogged");

            migrationBuilder.CreateIndex(
                name: "IX_JobAlerts_DateCreated",
                table: "JobAlerts",
                column: "DateCreated");

            migrationBuilder.Sql(@"
/* Generates data in the JobSeekerStats table for a 1-week period.
 *
 * Created by Mike Olund <mike@oxd.com>
 * April 18, 2020
 *
 * NOTE:
 * Stored procedures and functions are updated using code-first migrations. 
 * In order to keep localdev, dev, test and prod environments in sync,
 * they should never be modified directly in the sql database (unless 
 * you don't mind having your changes wiped out by a future release).
 * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
 */
ALTER PROCEDURE [dbo].[usp_GenerateJobSeekerStats] 
(
	@WeekEndDate DATETIME
)
AS 

BEGIN

DECLARE @StartDate DATETIME;
DECLARE @EndDatePlus1 DATETIME;
DECLARE @PeriodId INT;
DECLARE @TableName NVARCHAR(25);

SET @TableName = 'JobSeekerStats';

-- Get the WeeklyPeriod record
SELECT @StartDate = WeekStartDate, @PeriodId = Id
FROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;

SET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);

--- Check if a ReportPersistenceControl record exists.  
--- Delete if it is a TotalToDate record and it's more than 10 minutes old
IF EXISTS (SELECT * FROM ReportPersistenceControl 
			WHERE TableName = @TableName 
			AND WeeklyPeriodId = @PeriodId 
			AND DateCalculated < @EndDatePlus1
			AND DateCalculated < DATEADD(MINUTE,-10,GETDATE())) 
BEGIN 
	DELETE FROM ReportPersistenceControl 
	WHERE TableName = @TableName AND WeeklyPeriodId = @PeriodId;
	-- also delete associated record from JobSeekerStats
	DELETE FROM JobSeekerStats WHERE WeeklyPeriodId = @PeriodId;
END

BEGIN TRY

	IF NOT EXISTS (SELECT * FROM ReportPersistenceControl 
				   WHERE WeeklyPeriodId = @PeriodId 
				   AND TableName = @TableName)
	BEGIN 
		-- Store UserRegions in a Table Variable
		DECLARE @JobSeekerData TABLE (
			AspNetUserId NVARCHAR(450) PRIMARY KEY,
			RegionId INT,
			DateRegistered DATETIME2(7) NOT NULL,
			AccountStatus SMALLINT NOT NULL,
			EmailConfirmed BIT NOT NULL,
			IsApprentice BIT NOT NULL,
			IsIndigenousPerson BIT NOT NULL,
			IsMatureWorker BIT NOT NULL,
			IsNewImmigrant BIT NOT NULL,
			IsPersonWithDisability BIT NOT NULL,
			IsStudent BIT NOT NULL,
			IsVeteran BIT NOT NULL,
			IsVisibleMinority BIT NOT NULL,
			IsYouth BIT NOT NULL
		);

		INSERT INTO @JobSeekerData
		SELECT AspnetUserId, 
		(CASE WHEN RegionId IS NOT NULL THEN RegionId 
			  WHEN CountryId = 37 AND ProvinceId <> 2 THEN -1
			  WHEN (CountryId IS NOT NULL AND CountryId <> 37) THEN -2
			  ELSE 0 END) AS RegionId
			  ,DateRegistered
			  ,AccountStatus
			  ,EmailConfirmed
			  ,IsApprentice
			  ,IsIndigenousPerson
			  ,IsMatureWorker
			  ,IsNewImmigrant
			  ,IsPersonWithDisability
			  ,IsStudent
			  ,IsVeteran
			  ,IsVisibleMinority
			  ,IsYouth
		FROM dbo.tvf_GetJobSeekersForDate(@EndDatePlus1) js
		LEFT OUTER JOIN Locations l ON l.LocationId = js.LocationId

		-- insert a record into ReportPersistenceControl
		INSERT INTO ReportPersistenceControl (WeeklyPeriodId, TableName, DateCalculated, IsTotalToDate)
		SELECT @PeriodId AS WeeklyPeriodId, @TableName AS Report, 
			GETDATE() AS DateCalculated, 
			(CASE WHEN @EndDatePlus1 > GETDATE() THEN 1 ELSE 0 END) AS IsTotalToDate;

		-- ACCOUNTS BY STATUS

		--New Registrations
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'REGD', RegionId, COUNT(*) 
		FROM @JobSeekerData
		WHERE DateRegistered >= @StartDate AND DateRegistered < @EndDatePlus1 
        GROUP BY RegionId;

		--Awaiting Email Activation: This is the total at the end of the selected period. 
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'CFEM', RegionId, COUNT(DISTINCT je.AspNetUserId) 
		FROM JobSeekerEventLog je
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = je.AspNetUserId
		WHERE EventTypeId = 3 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1
		GROUP BY jd.RegionId;

		-- Get the net number of new unactivated accounts for the period
		-- by subtracting new email confirmations from new registrations.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT WeeklyPeriodId,'NOAC' AS [LabelKey],[RegionId], SUM([Value]) AS [Value]
		FROM (
			SELECT WeeklyPeriodId,[LabelKey],[RegionId],[Value] 
			FROM JobSeekerStats 
			WHERE WeeklyPeriodId = @PeriodId AND LabelKey = 'REGD'
			UNION
			SELECT WeeklyPeriodId,[LabelKey],[RegionId],-1 * [Value]
			FROM JobSeekerStats 
			WHERE WeeklyPeriodId = @PeriodId AND LabelKey = 'CFEM'
		) AS REGD_CFEM
		GROUP BY WeeklyPeriodId,[RegionId]

		-- Get statistics from the JobSeekerEventLog

		--Deactivated: This is accounts deactivated for this period.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'DEAC', RegionId, COUNT(DISTINCT je.AspNetUserId) 
		FROM JobSeekerEventLog je
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = je.AspNetUserId
		WHERE EventTypeId = 4 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1
		GROUP BY jd.RegionId;

		--Deleted: This is total account deleted for this period.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'DEL', RegionId, COUNT(DISTINCT je.AspNetUserId) 
		FROM JobSeekerEventLog je
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = je.AspNetUserId
		WHERE EventTypeId = 6 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1
		GROUP BY jd.RegionId;


		-- JOB SEEKER EMPLOYMENT GROUPS

		--Employment Groups: Total number of accounts

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'APPR', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsApprentice = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'INDP', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsIndigenousPerson = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'MAT', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsMatureWorker = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'IMMG', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsNewImmigrant = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'PWD', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsPersonWithDisability = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'STUD', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsStudent = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'VET', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsVeteran = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'VMIN', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsVisibleMinority = 1
        GROUP BY RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
        SELECT @PeriodId, 'YTH', RegionId, COUNT(Distinct AspNetUserId) 
		FROM @JobSeekerData
		WHERE IsYouth = 1
        GROUP BY RegionId;				

		-- ACCOUNT ACTIVITY

		-- Get statistics from the JobSeekerEventLog
		-- Logins: This is total number of times users succesfully logged in for this period.

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'LOGN', RegionId, COUNT(DISTINCT je.AspNetUserId) 
		FROM JobSeekerEventLog je
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = je.AspNetUserId
		WHERE EventTypeId = 1 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1
		GROUP BY jd.RegionId;

		--Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: 
		--These are total number of accounts, not new registrations.

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'ALRT', RegionId, COUNT(DISTINCT ja.AspNetUserId) 
		FROM JobAlerts ja
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = ja.AspNetUserId
		WHERE DateCreated < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= @EndDatePlus1)
		GROUP BY jd.RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'CAPR', jd.RegionId, COUNT(DISTINCT sc.AspNetUserId)
		FROM SavedCareerProfiles sc
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = sc.AspNetUserId
		WHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= @EndDatePlus1)
		GROUP BY jd.RegionId;

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'INPR', RegionId, COUNT(DISTINCT si.AspNetUserId) 
		FROM SavedIndustryProfiles si
		INNER JOIN @JobSeekerData jd ON jd.AspNetUserId = si.AspNetUserId
		WHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= @EndDatePlus1)
		GROUP BY jd.RegionId;

	END

END TRY

BEGIN CATCH

	DELETE FROM ReportPersistenceControl 
	WHERE TableName = @TableName AND WeeklyPeriodId = @PeriodId;
	-- also delete associated record from JobSeekerStats
	DELETE FROM JobSeekerStats WHERE WeeklyPeriodId = @PeriodId;

	RETURN -1;
END CATCH

RETURN 0;

END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeeklyPeriods_WeekEndDate",
                table: "WeeklyPeriods");

            migrationBuilder.DropIndex(
                name: "IX_SavedIndustryProfiles_DateDeleted",
                table: "SavedIndustryProfiles");

            migrationBuilder.DropIndex(
                name: "IX_SavedIndustryProfiles_DateSaved",
                table: "SavedIndustryProfiles");

            migrationBuilder.DropIndex(
                name: "IX_SavedCareerProfiles_DateDeleted",
                table: "SavedCareerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_SavedCareerProfiles_DateSaved",
                table: "SavedCareerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerEventLog_DateLogged",
                table: "JobSeekerEventLog");

            migrationBuilder.DropIndex(
                name: "IX_JobAlerts_DateCreated",
                table: "JobAlerts");
        }
    }
}
