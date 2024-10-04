SELECT l.City
	,IsNull(r.Name, 'N/A') AS Region
	,Sum(j.PositionsAvailable) AS Vacancies
	,Count(*) AS Postings
FROM dbo.tvf_GetJobsForDate(@EndDate) j
INNER JOIN Locations l ON l.LocationId = j.LocationId
INNER JOIN Regions r ON r.Id = l.RegionId
WHERE j.DateFirstImported >= @StartDate
	AND j.DateFirstImported < @EndDate
	AND (
		@RegionId = 0
		OR l.RegionId = @RegionId
		)
	AND (
		@JobSourceId = 0
		OR j.JobSourceId = @JobSourceId
		)
GROUP BY l.City
	,IsNull(r.Name, 'N/A')
ORDER BY Sum(j.PositionsAvailable) DESC;

WITH JobData
AS (
	SELECT j.IndustryId
		,Sum(j.PositionsAvailable) AS Vacancies
		,Count(*) AS Postings
	FROM dbo.tvf_GetJobsForDate(@EndDate) j
	INNER JOIN Locations ll ON ll.LocationId = j.LocationId
	WHERE j.DateFirstImported >= @StartDate
		AND j.DateFirstImported < @EndDate
		AND (
			@RegionId = 0
			OR ll.RegionId = @RegionId
			)
		AND j.JobSourceId = 1
	GROUP BY j.IndustryId
	)
SELECT i.Id
	,i.Title AS Industry
	,ISNULL(Vacancies, 0) AS Vacancies
	,ISNULL(Postings, 0) AS Postings
FROM Industries i
LEFT OUTER JOIN JobData j ON j.IndustryId = i.Id
WHERE i.Id NOT IN (
		38
		,33
		)
ORDER BY Vacancies DESC
	,Postings DESC
	,Title;

WITH Jobs (
	JobId
	,PositionsAvailable
	,NocCodeId2021
	,JobSourceId
	)
AS (
	SELECT j.JobId
		,j.PositionsAvailable AS Vacancies
		,j.NocCodeId2021
		,j.JobSourceId
	FROM dbo.tvf_GetJobsForDate(@EndDate) j
	WHERE j.DateFirstImported >= @StartDate
		AND j.DateFirstImported < @EndDate
		AND (
			@JobSourceId = 0
			OR j.JobSourceId = @JobSourceId
			)
	)
SELECT nc.Code AS NocCode2021
	,nc.Title AS NocTitle
	,IsNull(Sum(j.PositionsAvailable), 0) AS Vacancies
	,Count(j.JobId) AS Postings
FROM NocCodes2021 nc
LEFT OUTER JOIN Jobs j ON j.NocCodeId2021 = nc.Id {categoryPredicate}
GROUP BY nc.Code
	,nc.Title
ORDER BY Sum(j.PositionsAvailable) DESC
	,Count(j.JobId) DESC
	,nc.Code;

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.FiscalYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= GetDate()
		AND (
			r.IsHidden = 0
			OR r.Id IN (
				0
				,- 4
				,- 5
				)
			)
	)
	,jobdata
AS (
	SELECT jr.RegionId
		,wp.FiscalYear
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	WHERE @JobSourceId = 0
		OR jr.JobSourceId = @JobSourceId
	GROUP BY jr.RegionId
		,wp.FiscalYear
	)
SELECT d.Label
	,d.FiscalYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,FiscalYear;

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= GetDate()
		AND (
			r.IsHidden = 0
			OR r.Id IN (
				0
				,- 4
				,- 5
				)
			)
	)
	,jobdata
AS (
	SELECT jr.RegionId
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	WHERE @JobSourceId = 0
		OR jr.JobSourceId = @JobSourceId
	GROUP BY jr.RegionId
		,wp.CalendarMonth
		,wp.CalendarYear
	)
SELECT d.Label
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= GetDate()
		AND (
			r.IsHidden = 0
			OR r.Id IN (
				0
				,- 4
				,- 5
				)
			)
	)
	,jobdata
AS (
	SELECT jr.RegionId
		,jr.WeeklyPeriodId
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN WeeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	WHERE @JobSourceId = 0
		OR jr.JobSourceId = @JobSourceId
	GROUP BY jr.RegionId
		,jr.WeeklyPeriodId
	)
SELECT d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

WITH dimensions
AS (
	SELECT DISTINCT js.Id AS JobSourceId
		,js.GroupName AS Label
		,wp.FiscalYear
	FROM WeeklyPeriods wp
		,JobSources js
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= GetDate()
	)
	,jobdata
AS (
	SELECT jr.JobSourceId
		,wp.FiscalYear
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	GROUP BY jr.JobSourceId
		,wp.FiscalYear
	)
SELECT d.Label
	,d.FiscalYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,FiscalYear;

WITH dimensions
AS (
	SELECT DISTINCT js.Id AS JobSourceId
		,js.GroupName AS Label
		,wp.CalendarMonth
		,wp.CalendarYear
	FROM WeeklyPeriods wp
		,JobSources js
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= GetDate()
	)
	,jobdata
AS (
	SELECT jr.JobSourceId
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	GROUP BY jr.JobSourceId
		,wp.CalendarMonth
		,wp.CalendarYear
	)
SELECT d.Label
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

WITH dimensions
AS (
	SELECT DISTINCT js.Id AS JobSourceId
		,js.GroupName AS Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
	FROM WeeklyPeriods wp
		,JobSources js
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= GetDate()
	)
	,jobdata
AS (
	SELECT jr.JobSourceId
		,jr.WeeklyPeriodId
		,Sum(jr.PositionsAvailable) AS Vacancies
		,Sum(jr.JobPostings) AS Postings
	FROM JobStats jr
	INNER JOIN WeeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
	GROUP BY jr.JobSourceId
		,jr.WeeklyPeriodId
	)
SELECT d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Vacancies, 0) AS Vacancies
	,IsNull(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

WITH dimensions
AS (
	SELECT DISTINCT r.[Key]
		,r.[Label]
		,wp.FiscalYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= GetDate()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,wp.FiscalYear
		,Sum(jss.[Value]) AS [Value]
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
	WHERE rm.IsTotal = 0
		AND (
			@RegionId IS NULL
			OR jss.RegionId = @RegionId
			)
	GROUP BY jss.LabelKey
		,wp.FiscalYear

	UNION

	SELECT jss.LabelKey
		,wp.FiscalYear
		,Sum(jss.[Value]) AS [Value]
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
	WHERE rm.IsTotal = 1
		AND (
			wp.IsEndOfFiscalYear = 1
			OR wp.Id = @CurrentPeriodId
			)
		AND (
			@RegionId IS NULL
			OR jss.RegionId = @RegionId
			)
	GROUP BY jss.LabelKey
		,wp.FiscalYear
	)
SELECT d.[Key] AS LabelKey
	,d.Label
	,d.IsTotal
	,d.FiscalYear
	,IsNull(jd.[Value], 0) AS [Value]
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.[Key] = jd.LabelKey
ORDER BY d.Label
	,FiscalYear;

WITH dimensions
AS (
	SELECT DISTINCT r.[Key]
		,r.[Label]
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= GetDate()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jss.[Value]) AS [Value]
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
	WHERE rm.IsTotal = 0
		AND (
			@RegionId IS NULL
			OR jss.RegionId = @RegionId
			)
	GROUP BY jss.LabelKey
		,wp.CalendarMonth
		,wp.CalendarYear

	UNION

	SELECT jss.LabelKey
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jss.[Value]) AS [Value]
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
	WHERE rm.IsTotal = 1
		AND (
			wp.IsEndOfMonth = 1
			OR wp.Id = @CurrentPeriodId
			)
		AND (
			@RegionId IS NULL
			OR jss.RegionId = @RegionId
			)
	GROUP BY jss.LabelKey
		,wp.CalendarMonth
		,wp.CalendarYear
	)
SELECT d.[Key] AS LabelKey
	,d.Label
	,d.IsTotal
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.[Value], 0) AS [Value]
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.[Key] = jd.LabelKey
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

WITH dimensions
AS (
	SELECT DISTINCT r.[Key]
		,r.[Label]
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= GetDate()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,jss.WeeklyPeriodId
		,SUM(jss.[Value]) AS [Value]
	FROM JobSeekerStats jss
	INNER JOIN WeeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	WHERE @RegionId IS NULL
		OR jss.RegionId = @RegionId
	GROUP BY jss.LabelKey
		,jss.WeeklyPeriodId
	)
SELECT d.[Key] AS LabelKey
	,d.Label
	,d.IsTotal
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.[Value], 0) AS [Value]
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.[Key] = jd.LabelKey
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

WITH UsersWithJobAlerts
AS (
	SELECT DISTINCT AspNetUserId
	FROM JobAlerts
	WHERE IsDeleted = 0
	)
	,UsersWithCareerProfiles
AS (
	SELECT DISTINCT AspNetUserId
	FROM SavedCareerProfiles
	WHERE IsDeleted = 0
	)
	,UsersWithIndustryProfiles
AS (
	SELECT DISTINCT AspNetUserId
	FROM SavedIndustryProfiles
	WHERE IsDeleted = 0
	)
	,UsersWithSavedJobs
AS (
	SELECT DISTINCT AspNetUserId
	FROM SavedJobs
	WHERE IsDeleted = 0
	)
SELECT TOP (@MaxRows) u.Id
	,u.Email
	,u.FirstName
	,u.LastName
	,u.AccountStatus
	,l.City
	,r.[Name] AS Region
	,(
		CASE
			WHEN u.CountryId = 37
				AND u.ProvinceId = 2
				AND u.LocationId IS NULL
				THEN NULL
			ELSE c.[Name]
			END
		) AS Country
	,(
		CASE
			WHEN u.CountryId = 37
				AND u.ProvinceId = 2
				AND u.LocationId IS NULL
				THEN NULL
			ELSE p.[Name]
			END
		) AS Province
	,u.DateRegistered
	,u.LastModified
	,f.IsApprentice
	,f.IsIndigenousPerson
	,f.IsMatureWorker
	,f.IsNewImmigrant
	,f.IsPersonWithDisability
	,f.IsStudent
	,f.IsVeteran
	,f.IsVisibleMinority
	,f.IsYouth
	,(
		CASE
			WHEN ja.AspNetUserId IS NOT NULL
				THEN 1
			ELSE 0
			END
		) AS HasJobAlerts
	,(
		CASE
			WHEN cp.AspNetUserId IS NOT NULL
				THEN 1
			ELSE 0
			END
		) AS HasSavedCareerProfiles
	,(
		CASE
			WHEN ip.AspNetUserId IS NOT NULL
				THEN 1
			ELSE 0
			END
		) AS HasSavedIndustryProfiles
	,(
		CASE
			WHEN sj.AspNetUserId IS NOT NULL
				THEN 1
			ELSE 0
			END
		) AS HasSavedJobs
FROM AspNetUsers u
LEFT OUTER JOIN Locations l ON l.LocationId = u.LocationId
LEFT OUTER JOIN Regions r ON r.Id = l.RegionId
LEFT OUTER JOIN Provinces p ON p.ProvinceId = u.ProvinceId
LEFT OUTER JOIN Countries c ON c.Id = u.CountryId
LEFT OUTER JOIN JobSeekerFlags f ON f.AspNetUserId = u.Id
LEFT OUTER JOIN UsersWithJobAlerts ja ON ja.AspNetUserId = u.Id
LEFT OUTER JOIN UsersWithCareerProfiles cp ON cp.AspNetUserId = u.Id
LEFT OUTER JOIN UsersWithIndustryProfiles ip ON ip.AspNetUserId = u.Id
LEFT OUTER JOIN UsersWithSavedJobs sj ON sj.AspNetUserId = u.Id
WHERE (
		@StartDate IS NULL
		OR u.DateRegistered >= @StartDate
		)
	AND (
		@EndDate IS NULL
		OR u.DateRegistered <= @EndDate
		)
ORDER BY AccountStatus
	,DateRegistered DESC;

SELECT COUNT(*)
FROM AspNetUsers u
WHERE (
		@StartDate IS NULL
		OR u.DateRegistered >= @StartDate
		)
	AND (
		@EndDate IS NULL
		OR u.DateRegistered <= @EndDate
		);

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.FiscalYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= GetDate()
		AND r.Id NOT IN (
			0
			,- 4
			,- 5
			)
	)
	,jobseekerdata
AS (
	SELECT jsl.RegionId
		,wp.FiscalYear
		,Sum(jsl.Value) AS Value
	FROM JobSeekerStats jsl
	INNER JOIN weeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
	WHERE LabelKey = 'REGD'
	GROUP BY jsl.RegionId
		,wp.FiscalYear
	)
SELECT d.Label
	,d.FiscalYear
	,IsNull(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,FiscalYear;

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= GetDate()
		AND r.Id NOT IN (
			0
			,- 4
			,- 5
			)
	)
	,jobseekerdata
AS (
	SELECT jsl.RegionId
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jsl.Value) AS Value
	FROM JobSeekerStats jsl
	INNER JOIN weeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
	WHERE LabelKey = 'REGD'
	GROUP BY jsl.RegionId
		,wp.CalendarMonth
		,wp.CalendarYear
	)
SELECT d.Label
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.[Name] AS Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= GetDate()
		AND r.Id NOT IN (
			0
			,- 4
			,- 5
			)
	)
	,jobseekerdata
AS (
	SELECT jsl.RegionId
		,jsl.WeeklyPeriodId
		,jsl.Value
	FROM JobSeekerStats jsl
	INNER JOIN WeeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
	WHERE LabelKey = 'REGD'
	)
SELECT d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear
	,IsNull(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

UPDATE ImportedJobsWanted SET DateLastSeen = GetDate() WHERE JobId IN ({string.Join(',', jobIds)});

UPDATE [ImportedJobsFederal] SET [ReIndexNeeded] = 0 WHERE @IdList LIKE '%;' + Convert(nvarchar(20),[JobId]) + ';%';

UPDATE [ImportedJobsWanted] SET [ReIndexNeeded] = 0 WHERE @IdList LIKE '%;' + Convert(nvarchar(20),[JobId]) + ';%';

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
CREATE FUNCTION [dbo].[tvf_GetJobSeekersForDate](
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
);

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
CREATE FUNCTION [dbo].[tvf_GetJobsForDate](
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
);


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
CREATE PROCEDURE [dbo].[usp_GenerateJobSeekerStats]
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
--- Delete if it is a TotalToDate record and it's more than 12 hours old
IF EXISTS (SELECT * FROM ReportPersistenceControl
			WHERE TableName = @TableName
			AND WeeklyPeriodId = @PeriodId
			AND DateCalculated < @EndDatePlus1
			AND DateCalculated < DATEADD(HOUR,-12,GETDATE()))
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
			DateRegistered DATETIME2(7) NULL,
			AccountStatus SMALLINT NULL,
			EmailConfirmed BIT NULL,
			IsApprentice BIT NULL,
			IsIndigenousPerson BIT NULL,
			IsMatureWorker BIT NULL,
			IsNewImmigrant BIT NULL,
			IsPersonWithDisability BIT NULL,
			IsStudent BIT NULL,
			IsVeteran BIT NULL,
			IsVisibleMinority BIT NULL,
			IsYouth BIT NULL
		);

		IF @EndDatePlus1 < GETDATE()
			BEGIN
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
			END
		ELSE
			BEGIN
				INSERT INTO @JobSeekerData
				SELECT js.Id AS AspNetUserId,
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
				FROM AspNetUsers js
				LEFT OUTER JOIN JobSeekerFlags jf ON jf.AspNetUserId = js.Id
				LEFT OUTER JOIN Locations l ON l.LocationId = js.LocationId
			END

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
		-- Logins: This is total number of times users successfully logged in for this period.

		INSERT INTO dbo.JobSeekerStats
			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
		SELECT @PeriodId, 'LOGN', RegionId, COUNT(je.AspNetUserId)
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

END;


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
CREATE PROCEDURE [dbo].[usp_GenerateJobStats]
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

END;
