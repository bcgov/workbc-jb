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
    ///     Dapper queries for the Job Seekers by Location Report
    /// </summary>
    public class JobSeekersByLocationReportQueryService : QueryServiceBase
    {
        internal JobSeekersByLocationReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the yearly query
        /// </summary>
        public async Task<IList<JobSeekersByLocationResult>> RunYearlyAsync(int startYear, int endYear)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS
                    (
                        SELECT DISTINCT r.Id AS RegionId, r.[Name] AS Label, wp.FiscalYear,
                            r.ListOrder AS SortOrder
                        FROM WeeklyPeriods wp, Regions r
                        WHERE wp.FiscalYear >= @StartYear AND wp.FiscalYear <= @EndYear
                            AND wp.WeekStartDate <= GetDate() AND r.Id NOT IN (0,-4,-5)
                    ),
                    jobseekerdata AS
                    (
	                    SELECT jsl.RegionId,
		                    wp.FiscalYear,
		                    Sum(jsl.Value) AS Value
	                    FROM JobSeekerStats jsl
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
                        WHERE LabelKey = 'REGD'
	                    GROUP BY jsl.RegionId, wp.FiscalYear
                    )
                    SELECT d.Label,
	                    d.FiscalYear,
	                    IsNull(jd.Value,0) AS Users,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.FiscalYear = jd.FiscalYear
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, FiscalYear";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekersByLocationResult>(sql,
                    new
                    {
                        StartYear = startYear,
                        EndYear = endYear
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the monthly query
        /// </summary>
        public async Task<IList<JobSeekersByLocationResult>> RunMonthlyAsync(DateTime startDate, DateTime endDate)
        {
            // set up the query
            var sql =
                @"WITH dimensions AS
                    (
	                    SELECT DISTINCT r.Id AS RegionId, r.[Name] AS Label,
                            wp.CalendarMonth, wp.CalendarYear, r.ListOrder AS SortOrder
	                    FROM WeeklyPeriods wp, Regions r
	                    WHERE wp.WeekStartDate >= @StartDate AND wp.WeekEndDate <= @EndDate
                            AND wp.WeekStartDate <= GetDate() AND r.Id NOT IN (0,-4,-5)
                    ),
                    jobseekerdata AS
                    (
	                    SELECT jsl.RegionId,
		                    wp.CalendarMonth,
		                    wp.CalendarYear,
		                    Sum(jsl.Value) AS Value
	                    FROM JobSeekerStats jsl
		                    INNER JOIN weeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
                        WHERE LabelKey = 'REGD'
	                    GROUP BY jsl.RegionId, wp.CalendarMonth, wp.CalendarYear
                    )
                    SELECT d.Label,
	                    d.CalendarMonth,
	                    d.CalendarYear,
	                    IsNull(jd.Value,0) AS Users,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.CalendarMonth = jd.CalendarMonth
	                    AND d.CalendarYear = jd.CalendarYear
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, CalendarYear, d.CalendarMonth";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekersByLocationResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Runs the weekly query
        /// </summary>
        public async Task<IList<JobSeekersByLocationResult>> RunWeeklyAsync(int year, int month)
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
                            AND wp.WeekStartDate <= GetDate() AND r.Id NOT IN (0,-4,-5)
                    ),
                    jobseekerdata AS
                    (
	                    SELECT jsl.RegionId,
		                    jsl.WeeklyPeriodId,
		                    jsl.Value
	                    FROM JobSeekerStats jsl
		                    INNER JOIN WeeklyPeriods wp ON wp.Id = jsl.WeeklyPeriodId
                        WHERE LabelKey = 'REGD'
                    )
                    SELECT d.Label,
	                    d.WeekOfMonth,
                        d.CalendarMonth,
                        d.CalendarYear,
	                    IsNull(jd.Value,0) AS Users,
                        d.SortOrder
                    FROM dimensions d
                    LEFT OUTER JOIN jobseekerdata jd ON d.WeeklyPeriodId = jd.WeeklyPeriodId
	                    AND d.RegionId = jd.RegionId
                    ORDER BY d.Label, d.WeekOfMonth, d.CalendarMonth,  d.CalendarYear";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekersByLocationResult>(sql,
                    new
                    {
                        Year = year,
                        Month = month
                    }
                )).ToList();
            }
        }

        /// <summary>
        ///     Gets the total number of registered users at the end of the day on the date specified
        /// </summary>
        public async Task<int> GetTotalJobSeekerCount(DateTime endDate)
        {
            // add 1 day to the endDate so the entire date selected is included in the result
            endDate = endDate.AddDays(1);

            var sql = @"SELECT Count(*) FROM [dbo].[tvf_GetJobSeekersForDate](@EndDate)
                        WHERE AccountStatus <> 99";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return await conn.ExecuteScalarAsync<int>(sql, new {EndDate = endDate});
            }
        }
    }
}