using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSekerStatsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER PROCEDURE [dbo].[usp_PopulateJobSeekerStats] 
(
	@WeekEndDate DATETIME
)
AS 

BEGIN

BEGIN TRANSACTION;

DECLARE @StartDate DATETIME;
DECLARE @EndDatePlus1 DATETIME;
DECLARE @WeekEndDateEOD DATETIME;
DECLARE @PeriodId INT;
DECLARE @ReportName NVARCHAR(25);

SET @ReportName = 'JobSeekerStats';

BEGIN TRY
	-- Get the WeeklyPeriod record
	SELECT @StartDate = WeekStartDate, @PeriodId = Id
	FROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;

	-- Get the End Date (the day after the WeekEndDate so we can include things that happened since midnight)
	SET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);
	SET @WeekEndDateEOD = DATEADD(MILLISECOND,-3,@EndDatePlus1);

	--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record
	IF EXISTS (SELECT * FROM ReportPersistenceControl 
			   WHERE Report = @ReportName 
			   AND WeeklyPeriodId = @PeriodId 
			   AND DateCalculated < DATEADD(HOUR, 48, @WeekEndDate)) 
	BEGIN 
		DELETE FROM ReportPersistenceControl 
		WHERE Report = @ReportName AND WeeklyPeriodId = @PeriodId;
		-- also delete associated record from JobSeekerStats
		DELETE FROM JobSeekerStats WHERE WeeklyPeriodId = @PeriodId;
	END

	IF NOT EXISTS (SELECT * FROM ReportPersistenceControl 
				   WHERE WeeklyPeriodId = @PeriodId 
				   AND Report = @ReportName)
	BEGIN 
		-- insert a record intoo ReportPersistenceControl
		INSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)
		SELECT @PeriodId AS WeeklyPeriodId, @ReportName AS Report, 
			GETDATE() AS DateCalculated, 
			(CASE WHEN DATEADD(HOUR,48,@WeekEndDate) > GETDATE() THEN 1 ELSE 0 END) AS IsTotalToDate;


		-- ACCOUNTS BY STATUS

		--New Registrations
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'REGD', COUNT(*) 
		FROM [dbo].[tvf_GetJobSeekersForDate](@EndDatePlus1)
		WHERE DateRegistered >= @StartDate AND DateRegistered < @EndDatePlus1 

		--Awaiting Email Activation: This is the total at the end of the selected period. 
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'NOAC', COUNT(*) 
		FROM [dbo].[tvf_GetJobSeekersForDate](@EndDatePlus1)
		WHERE DateRegistered >= @StartDate AND DateRegistered < @EndDatePlus1 AND AccountStatus = 4;

		-- Get statistics from the JobSeekerEventLog

		--Deactivated: This is accounts deactivated for this period.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'DEAC', COUNT(DISTINCT AspNetUserId) 
		FROM JobSeekerEventLog
		WHERE EventTypeId = 4 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;

		--Deleted: This is total account deleted for this period.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'DEL', COUNT(DISTINCT AspNetUserId) 
		FROM JobSeekerEventLog
		WHERE EventTypeId = 6 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;


		-- JOB SEEKER EMPLOYMENT GROUPS

		--Employment Groups: Is this for new registrations or total number of accounts? 
		--This is total number of accounts
		DECLARE @IsApprentice INT;
		DECLARE @IsIndigenousPerson INT;
		DECLARE @IsMatureWorker INT;
		DECLARE @IsNewImmigrant INT;
		DECLARE @IsPersonWithDisability INT;
		DECLARE @IsStudent INT;
		DECLARE @IsVeteran INT;
		DECLARE @IsVisibleMinority INT;
		DECLARE @IsYouth INT;

		SELECT @IsApprentice = SUM(CAST(IsApprentice AS INT)), 
			@IsIndigenousPerson = SUM(CAST(IsIndigenousPerson AS INT)), 
			@IsMatureWorker = SUM(CAST(IsMatureWorker AS INT)),
			@IsNewImmigrant = SUM(CAST(IsNewImmigrant AS INT)),
			@IsPersonWithDisability = SUM(CAST(IsPersonWithDisability AS INT)),
			@IsStudent = SUM(CAST(IsStudent AS INT)),
			@IsVeteran = SUM(CAST(IsVeteran AS INT)),
			@IsVisibleMinority = SUM(CAST(IsVisibleMinority AS INT)),
			@IsYouth = SUM(CAST(IsYouth AS INT))
		FROM [dbo].[tvf_GetJobSeekersForDate](@EndDatePlus1);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'APPR',@IsApprentice);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'INDP',@IsIndigenousPerson);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'MAT',@IsMatureWorker);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'IMMG',@IsNewImmigrant);
		
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'PWD',@IsPersonWithDisability);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'STUD',@IsStudent);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'VET',@IsVeteran);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'VMIN',@IsVisibleMinority);
		
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		VALUES (@PeriodId,'YTH',@IsYouth);


		-- ACCOUNT ACTIVITY

		-- Get statistics from the JobSeekerEventLog
		-- Logins: This is total number of times users succesfully logged in for this period.
		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'LOGN', COUNT(*) 
		FROM JobSeekerEventLog
		WHERE EventTypeId = 1 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;

		--Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: 
		--These are total number of accounts, not new registrations.

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'ALRT', COUNT(DISTINCT AspNetUserId) 
		FROM JobAlerts 
		WHERE DateCreated < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'CAPR', COUNT(DISTINCT AspNetUserId) 
		FROM SavedCareerProfiles 
		WHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[ReportMetadataKey],[Value])
		SELECT @PeriodId, 'INPR', COUNT(DISTINCT AspNetUserId) 
		FROM SavedIndustryProfiles 
		WHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);

	END

	COMMIT TRANSACTION;

END TRY

BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END
	RETURN -1;
END CATCH

RETURN 0;

END
");

            migrationBuilder.Sql("DELETE FROM ReportPersistenceControl WHERE Report = 'JobSeekerStats'");
            migrationBuilder.Sql("DELETE FROM JobSeekerStats");
            migrationBuilder.Sql("UPDATE ReportMetadata SET IsTotal = 0 WHERE [Key] = 'NOAC'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
