using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;

namespace WorkBC.Admin.Areas.Reports.Data.QueryServices
{
    /// <summary>
    ///     Dapper queries for the Job Seeker Account Report
    /// </summary>
    public class JobSeekerAccountReportQueryService : QueryServiceBase
    {
        internal JobSeekerAccountReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the yearly query
        /// </summary>
        public async Task<IList<JobSeekerAccountResult>> RunYearlyAsync(int startYear, int endYear, int currentPeriodId, int? regionId = null)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS 
                    (
                        SELECT DISTINCT r.[Key], r.[Label],
                            wp.FiscalYear, r.IsTotal
                        FROM WeeklyPeriods wp, JobSeekerStatLabels r
                        WHERE wp.FiscalYear >= @StartYear AND wp.FiscalYear <= @EndYear
                            AND wp.WeekStartDate <= GetDate()
                    ),
                    jobseekerdata AS 
                    (
	                    SELECT jss.LabelKey,
		                    wp.FiscalYear, 
		                    Sum(jss.[Value]) AS [Value]
	                    FROM JobSeekerStats jss
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
                            INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
                        WHERE rm.IsTotal = 0 AND (@RegionId IS NULL OR jss.RegionId = @RegionId)
	                    GROUP BY jss.LabelKey, wp.FiscalYear

                        UNION

	                    SELECT jss.LabelKey,
		                    wp.FiscalYear, 
		                    Sum(jss.[Value]) AS [Value]
	                    FROM JobSeekerStats jss
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
                            INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
                        WHERE rm.IsTotal = 1 
                            AND (wp.IsEndOfFiscalYear = 1 OR wp.Id = @CurrentPeriodId)
                            AND (@RegionId IS NULL OR jss.RegionId = @RegionId)
                        GROUP BY jss.LabelKey, wp.FiscalYear
                    )
                    SELECT d.[Key] AS LabelKey,
                        d.Label,
                        d.IsTotal,
	                    d.FiscalYear,
	                    IsNull(jd.[Value],0) AS [Value]
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear 
	                    AND d.[Key] = jd.LabelKey
                    ORDER BY d.Label, FiscalYear";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekerAccountResult>(sql,
                    new
                    {
                        StartYear = startYear,
                        EndYear = endYear,
                        CurrentPeriodId = currentPeriodId,
                        RegionId = regionId
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the monthly query
        /// </summary>
        public async Task<IList<JobSeekerAccountResult>> RunMonthlyAsync(DateTime startDate, DateTime endDate, int currentPeriodId, int? regionId = null)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS 
                    (
	                    SELECT DISTINCT r.[Key], r.[Label],
                            wp.CalendarMonth, wp.CalendarYear, r.IsTotal
	                    FROM WeeklyPeriods wp, JobSeekerStatLabels r
	                    WHERE wp.WeekStartDate >= @StartDate AND wp.WeekEndDate <= @EndDate
                            AND wp.WeekStartDate <= GetDate()
                    ),
                    jobseekerdata AS 
                    (
	                    SELECT jss.LabelKey,
		                    wp.CalendarMonth, 
		                    wp.CalendarYear,
		                    Sum(jss.[Value]) AS [Value]
	                    FROM JobSeekerStats jss
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
							INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
						WHERE rm.IsTotal = 0 AND (@RegionId IS NULL OR jss.RegionId = @RegionId)
	                    GROUP BY jss.LabelKey, wp.CalendarMonth, wp.CalendarYear

						UNION

						SELECT jss.LabelKey,
		                    wp.CalendarMonth, 
		                    wp.CalendarYear,
		                    Sum(jss.[Value]) AS [Value]
	                    FROM JobSeekerStats jss
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
							INNER JOIN JobSeekerStatLabels rm ON rm.[Key] = jss.LabelKey
                        WHERE rm.IsTotal = 1 
							AND (wp.IsEndOfMonth = 1 OR wp.Id = @CurrentPeriodId)
							AND (@RegionId IS NULL OR jss.RegionId = @RegionId)
						GROUP BY jss.LabelKey, wp.CalendarMonth, wp.CalendarYear
                    )
                    SELECT d.[Key] AS LabelKey,
						d.Label,
                        d.IsTotal,
	                    d.CalendarMonth, 
	                    d.CalendarYear,
	                    IsNull(jd.[Value],0) AS [Value]
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth 
	                    AND d.CalendarYear = jd.CalendarYear 
	                    AND d.[Key] = jd.LabelKey
                    ORDER BY d.Label, CalendarYear, d.CalendarMonth";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekerAccountResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        CurrentPeriodId = currentPeriodId,
                        RegionId = regionId
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the weekly query
        /// </summary>
        public async Task<IList<JobSeekerAccountResult>> RunWeeklyAsync(int year, int month, int? regionId = null)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS 
                    (
	                    SELECT DISTINCT r.[Key], r.[Label], 
                            wp.WeekOfMonth, wp.Id AS WeeklyPeriodId, 
                            wp.CalendarMonth, wp.CalendarYear, r.IsTotal
	                    FROM WeeklyPeriods wp, JobSeekerStatLabels r
	                    WHERE  wp.CalendarYear = @Year AND wp.CalendarMonth = @Month 
                            AND wp.WeekStartDate <= GetDate()
					),
                    jobseekerdata AS 
                    (
	                    SELECT jss.LabelKey,
		                    jss.WeeklyPeriodId, 
		                    SUM(jss.[Value]) AS [Value]
	                    FROM JobSeekerStats jss
		                    INNER JOIN WeeklyPeriods wp ON wp.Id = jss.WeeklyPeriodId
						WHERE @RegionId IS NULL OR jss.RegionId = @RegionId
						GROUP BY jss.LabelKey, jss.WeeklyPeriodId
                    )
                    SELECT d.[Key] AS LabelKey,
					    d.Label,
                        d.IsTotal,
	                    d.WeekOfMonth,
                        d.CalendarMonth,
                        d.CalendarYear,
	                    IsNull(jd.[Value],0) AS [Value]
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	                    AND d.[Key] = jd.LabelKey
                    ORDER BY d.Label, d.WeekOfMonth, d.CalendarMonth,  d.CalendarYear
";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekerAccountResult>(sql,
                    new
                    {
                        Year = year,
                        Month = month,
                        RegionId = regionId
                    }
                )).ToList();
            }
        }
    }
}