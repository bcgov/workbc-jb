--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByCityReportQueryService.cs
--
SELECT l.City
	,Coalesce(r.Name, 'N/A') AS Region
	,Sum(j.PositionsAvailable) AS Vacancies
	,Count(*) AS Postings
FROM tvf_GetJobsForDate(@EndDate) j
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
	,Coalesce(r.Name, 'N/A')
ORDER BY Sum(j.PositionsAvailable) DESC;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByIndustryReportQueryService.cs
--
WITH JobData
AS (
	SELECT j.IndustryId
		,Sum(j.PositionsAvailable) AS Vacancies
		,Count(*) AS Postings
	FROM tvf_GetJobsForDate(@EndDate) j
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
	,Coalesce(Vacancies, 0) AS Vacancies
	,Coalesce(Postings, 0) AS Postings
FROM Industries i
LEFT OUTER JOIN JobData j ON j.IndustryId = i.Id
WHERE i.Id NOT IN (
		38
		,33
		)
ORDER BY Vacancies DESC
	,Postings DESC
	,Title;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByNocCodeReportQueryService.cs
--
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
	FROM tvf_GetJobsForDate(@EndDate) j
	WHERE j.DateFirstImported >= @StartDate
		AND j.DateFirstImported < @EndDate
		AND (
			@JobSourceId = 0
			OR j.JobSourceId = @JobSourceId
			)
	)
SELECT nc.Code AS NocCode2021
	,nc.Title AS NocTitle
	,Coalesce(Sum(j.PositionsAvailable), 0) AS Vacancies
	,Count(j.JobId) AS Postings
FROM NocCodes2021 nc
-- TODO Add {categoryPredicate} to the next line. --
LEFT OUTER JOIN Jobs j ON j.NocCodeId2021 = nc.Id
GROUP BY nc.Code
	,nc.Title
ORDER BY Sum(j.PositionsAvailable) DESC
	,Count(j.JobId) DESC
	,nc.Code;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByRegionReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.FiscalYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,FiscalYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByRegionReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsByRegionReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsBySourceReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT js.Id AS JobSourceId
		,js.GroupName AS Label
		,wp.FiscalYear
	FROM WeeklyPeriods wp
		,JobSources js
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,FiscalYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsBySourceReportQueryService.cs
--
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
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobsBySourceReportQueryService.cs
--
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
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Vacancies, 0) AS Vacancies
	,Coalesce(jd.Postings, 0) AS Postings
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.JobSourceId = jd.JobSourceId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekerAccountReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Key
		,r.Label
		,wp.FiscalYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= Now()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,wp.FiscalYear
		,Sum(jss.Value) AS Value
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.Key = jss.LabelKey
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
		,Sum(jss.Value) AS Value
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.Key = jss.LabelKey
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
SELECT d.Key AS LabelKey
	,d.Label
	,d.IsTotal
	,d.FiscalYear
	,Coalesce(jd.Value, 0) AS Value
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.Key = jd.LabelKey
ORDER BY d.Label
	,FiscalYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekerAccountReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Key
		,r.Label
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= Now()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,wp.CalendarMonth
		,wp.CalendarYear
		,Sum(jss.Value) AS Value
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.Key = jss.LabelKey
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
		,Sum(jss.Value) AS Value
	FROM JobSeekerStats jss
	INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	INNER JOIN JobSeekerStatLabels rm ON rm.Key = jss.LabelKey
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
SELECT d.Key AS LabelKey
	,d.Label
	,d.IsTotal
	,d.CalendarMonth
	,d.CalendarYear
	,Coalesce(jd.Value, 0) AS Value
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.Key = jd.LabelKey
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekerAccountReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Key
		,r.Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.IsTotal
	FROM WeeklyPeriods wp
		,JobSeekerStatLabels r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= Now()
	)
	,jobseekerdata
AS (
	SELECT jss.LabelKey
		,jss.WeeklyPeriodId
		,SUM(jss.Value) AS Value
	FROM JobSeekerStats jss
	INNER JOIN WeeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
	WHERE @RegionId IS NULL
		OR jss.RegionId = @RegionId
	GROUP BY jss.LabelKey
		,jss.WeeklyPeriodId
	)
SELECT d.Key AS LabelKey
	,d.Label
	,d.IsTotal
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear
	,Coalesce(jd.Value, 0) AS Value
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.Key = jd.LabelKey
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekerDetailReportQueryService.cs
--
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
SELECT DISTINCT u.Id
	,u.Email
	,u.FirstName
	,u.LastName
	,u.AccountStatus
	,l.City
	,r.Name AS Region
	,(
		CASE
			WHEN u.CountryId = 37
				AND u.ProvinceId = 2
				AND u.LocationId IS NULL
				THEN NULL
			ELSE c.Name
			END
		) AS Country
	,(
		CASE
			WHEN u.CountryId = 37
				AND u.ProvinceId = 2
				AND u.LocationId IS NULL
				THEN NULL
			ELSE p.Name
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
	,DateRegistered DESC
LIMIT @MaxRows;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekerDetailReportQueryService.cs
--
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

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekersByLocationReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.FiscalYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.FiscalYear >= @StartYear
		AND wp.FiscalYear <= @EndYear
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,FiscalYear;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekersByLocationReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.WeekStartDate >= @StartDate
		AND wp.WeekEndDate <= @EndDate
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth
	AND d.CalendarYear = jd.CalendarYear
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,CalendarYear
	,d.CalendarMonth;

--
-- src/WorkBC.Admin/Areas/Reports/Data/QueryServices/JobSeekersByLocationReportQueryService.cs
--
WITH dimensions
AS (
	SELECT DISTINCT r.Id AS RegionId
		,r.Name AS Label
		,wp.WeekOfMonth
		,wp.Id AS WeeklyPeriodId
		,wp.CalendarMonth
		,wp.CalendarYear
		,r.ListOrder AS SortOrder
	FROM WeeklyPeriods wp
		,Regions r
	WHERE wp.CalendarYear = @Year
		AND wp.CalendarMonth = @Month
		AND wp.WeekStartDate <= Now()
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
	,Coalesce(jd.Value, 0) AS Users
	,d.SortOrder
FROM dimensions d
LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	AND d.RegionId = jd.RegionId
ORDER BY d.Label
	,d.WeekOfMonth
	,d.CalendarMonth
	,d.CalendarYear;

--
-- src/WorkBC.Importers.Wanted/Services/XmlImportService.cs
--
UPDATE ImportedJobsWanted SET DateLastSeen = Now() WHERE JobId IN ({string.Join(',', jobIds)});

--
-- src/WorkBC.Indexers.Federal/Services/FederalIndexService.cs
--
UPDATE ImportedJobsFederal SET ReIndexNeeded = 0 WHERE @IdList LIKE '%;' + Convert(nvarchar(20),[JobId]) + ';%';

--
-- src/WorkBC.Indexers.Wanted/Services/WantedIndexService.cs
--
UPDATE ImportedJobsWanted SET ReIndexNeeded = 0 WHERE @IdList LIKE '%;' + Convert(nvarchar(20),[JobId]) + ';%';
