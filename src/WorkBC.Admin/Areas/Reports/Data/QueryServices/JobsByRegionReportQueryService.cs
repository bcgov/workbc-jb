using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;

namespace WorkBC.Admin.Areas.Reports.Data.QueryServices
{
    /// <summary>
    ///     Dapper queries for the Jobs by Region Report
    /// </summary>
    public class JobsByRegionReportQueryService : QueryServiceBase
    {
        internal JobsByRegionReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the yearly query
        /// </summary>
        public async Task<IList<JobStatsResult>> RunYearlyAsync(int startYear, int endYear, int jobSourceId)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS
                    (
                        SELECT DISTINCT r.Id AS RegionId, r.[Name] AS Label, wp.FiscalYear, r.ListOrder AS SortOrder
                        FROM WeeklyPeriods wp, Regions r
                        WHERE wp.FiscalYear >= @StartYear AND wp.FiscalYear <= @EndYear
                            AND wp.WeekStartDate <= GetDate()
                            AND (r.IsHidden = 0 OR r.Id IN (0,-4,-5))
                    ),
                    jobdata AS
                    (
	                    SELECT jr.RegionId,
		                    wp.FiscalYear,
		                    Sum(jr.PositionsAvailable) AS Vacancies,
		                    Sum(jr.JobPostings) AS Postings
	                    FROM JobStats jr
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
                        WHERE @JobSourceId = 0 OR jr.JobSourceId = @JobSourceId
	                    GROUP BY jr.RegionId, wp.FiscalYear
                    )
                    SELECT d.Label,
	                    d.FiscalYear,
	                    IsNull(jd.Vacancies,0) AS Vacancies,
	                    IsNull(jd.Postings,0) AS Postings,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobdata jd ON d.FiscalYear = jd.FiscalYear
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, FiscalYear";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
                    new
                    {
                        StartYear = startYear,
                        EndYear = endYear,
                        JobSourceId = jobSourceId
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the monthly query
        /// </summary>
        public async Task<IList<JobStatsResult>> RunMonthlyAsync(DateTime startDate, DateTime endDate,
            int jobSourceId)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS
                    (
	                    SELECT DISTINCT r.Id AS RegionId, r.[Name] AS Label,
                            wp.CalendarMonth, wp.CalendarYear, r.ListOrder AS SortOrder
	                    FROM WeeklyPeriods wp, Regions r
	                    WHERE wp.WeekStartDate >= @StartDate AND wp.WeekEndDate <= @EndDate
                            AND wp.WeekStartDate <= GetDate()
                            AND (r.IsHidden = 0 OR r.Id IN (0,-4,-5))
                    ),
                    jobdata AS
                    (
	                    SELECT jr.RegionId,
		                    wp.CalendarMonth,
		                    wp.CalendarYear,
		                    Sum(jr.PositionsAvailable) AS Vacancies,
		                    Sum(jr.JobPostings) AS Postings
	                    FROM JobStats jr
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
                        WHERE @JobSourceId = 0 OR jr.JobSourceId = @JobSourceId
	                    GROUP BY jr.RegionId, wp.CalendarMonth, wp.CalendarYear
                    )
                    SELECT d.Label,
	                    d.CalendarMonth,
	                    d.CalendarYear,
	                    IsNull(jd.Vacancies,0) AS Vacancies,
	                    IsNull(jd.Postings,0) AS Postings,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobdata jd ON d.CalendarMonth = jd.CalendarMonth
	                    AND d.CalendarYear = jd.CalendarYear
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, CalendarYear, d.CalendarMonth";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        JobSourceId = jobSourceId
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the weekly query
        /// </summary>
        public async Task<IList<JobStatsResult>> RunWeeklyAsync(int year, int month, int jobSourceId)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS
                    (
	                    SELECT DISTINCT r.Id AS RegionId, r.[Name] AS Label,
                            wp.WeekOfMonth, wp.Id AS WeeklyPeriodId,
                            wp.CalendarMonth, wp.CalendarYear, r.ListOrder AS SortOrder
	                    FROM WeeklyPeriods wp, Regions r
	                    WHERE  wp.CalendarYear = @Year AND wp.CalendarMonth = @Month
                            AND wp.WeekStartDate <= GetDate()
                            AND (r.IsHidden = 0 OR r.Id IN (0,-4,-5))
                    ),
                    jobdata AS
                    (
	                    SELECT jr.RegionId,
		                    jr.WeeklyPeriodId,
		                    Sum(jr.PositionsAvailable) AS Vacancies,
		                    Sum(jr.JobPostings) AS Postings
	                    FROM JobStats jr
		                    INNER JOIN WeeklyPeriods wp ON wp.Id = jr.WeeklyPeriodId
                        WHERE @JobSourceId = 0 OR jr.JobSourceId = @JobSourceId
	                    GROUP BY jr.RegionId, jr.WeeklyPeriodId
                    )
                    SELECT d.Label,
	                    d.WeekOfMonth,
                        d.CalendarMonth,
                        d.CalendarYear,
	                    IsNull(jd.Vacancies,0) AS Vacancies,
	                    IsNull(jd.Postings,0) AS Postings,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, d.WeekOfMonth, d.CalendarMonth, d.CalendarYear";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
                    new
                    {
                        Year = year,
                        Month = month,
                        JobSourceId = jobSourceId
                    }
                )).ToList();
            }
        }
    }
}