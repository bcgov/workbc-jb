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
    ///     Dapper queries for the Jobs by Source Report
    /// </summary>
    public class JobsBySourceReportQueryService : QueryServiceBase
    {
        internal JobsBySourceReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the yearly query
        /// </summary>
        public async Task<IList<JobStatsResult>> RunYearlyAsync(int startYear, int endYear)
        {
            // set up the query
            var sql = @"
WITH dimensions
AS (
  SELECT DISTINCT js.""Id"" AS ""JobSourceId""
    ,js.""GroupName"" AS ""Label""
    ,wp.""FiscalYear""
  FROM ""WeeklyPeriods"" wp
    ,""JobSources"" js
  WHERE wp.""FiscalYear"" >= @StartYear
    AND wp.""FiscalYear"" <= @EndYear
    AND wp.""WeekStartDate"" <= Now()
  )
  ,jobdata
AS (
  SELECT jr.""JobSourceId""
    ,wp.""FiscalYear""
    ,Sum(jr.""PositionsAvailable"") AS ""Vacancies""
    ,Sum(jr.""JobPostings"") AS ""Postings""
  FROM ""JobStats"" jr
  INNER JOIN ""WeeklyPeriods"" wp ON wp.""Id"" = jr.""WeeklyPeriodId""
  GROUP BY jr.""JobSourceId""
    ,wp.""FiscalYear""
  )
SELECT d.""Label""
  ,d.""FiscalYear""
  ,Coalesce(jd.""Vacancies"", 0) AS ""Vacancies""
  ,Coalesce(jd.""Postings"", 0) AS ""Postings""
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.""FiscalYear"" = jd.""FiscalYear""
  AND d.""JobSourceId"" = jd.""JobSourceId""
ORDER BY d.""Label""
  ,d.""FiscalYear"";
            ";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
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
        public async Task<IList<JobStatsResult>> RunMonthlyAsync(DateTime startDate, DateTime endDate)
        {
            // set up the query
            var sql = @"
WITH dimensions
AS (
  SELECT DISTINCT js.""Id"" AS ""JobSourceId""
    ,js.""GroupName"" AS ""Label""
    ,wp.""CalendarMonth""
    ,wp.""CalendarYear""
  FROM ""WeeklyPeriods"" wp
    ,""JobSources"" js
  WHERE wp.""WeekStartDate"" >= @StartDate
    AND wp.""WeekEndDate"" <= @EndDate
    AND wp.""WeekStartDate"" <= Now()
  )
  ,jobdata
AS (
  SELECT jr.""JobSourceId""
    ,wp.""CalendarMonth""
    ,wp.""CalendarYear""
    ,Sum(jr.""PositionsAvailable"") AS ""Vacancies""
    ,Sum(jr.""JobPostings"") AS ""Postings""
  FROM ""JobStats"" jr
  INNER JOIN ""WeeklyPeriods"" wp ON wp.""Id"" = jr.""WeeklyPeriodId""
  GROUP BY jr.""JobSourceId""
    ,wp.""CalendarMonth""
    ,wp.""CalendarYear""
  )
SELECT d.""Label""
  ,d.""CalendarMonth""
  ,d.""CalendarYear""
  ,Coalesce(jd.""Vacancies"", 0) AS ""Vacancies""
  ,Coalesce(jd.""Postings"", 0) AS ""Postings""
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.""CalendarMonth"" = jd.""CalendarMonth""
  AND d.""CalendarYear"" = jd.""CalendarYear""
  AND d.""JobSourceId"" = jd.""JobSourceId""
ORDER BY d.""Label""
  ,d.""CalendarYear""
  ,d.""CalendarMonth"";
            ";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
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
        public async Task<IList<JobStatsResult>> RunWeeklyAsync(int year, int month)
        {
            // set up the query
            var sql = @"
WITH dimensions
AS (
  SELECT DISTINCT js.""Id"" AS ""JobSourceId""
    ,js.""GroupName"" AS ""Label""
    ,wp.""WeekOfMonth""
    ,wp.""Id"" AS ""WeeklyPeriodId""
    ,wp.""CalendarMonth""
    ,wp.""CalendarYear""
  FROM ""WeeklyPeriods"" wp
    ,""JobSources"" js
  WHERE wp.""CalendarYear"" = @Year
    AND wp.""CalendarMonth"" = @Month
    AND wp.""WeekStartDate"" <= Now()
  )
  ,jobdata
AS (
  SELECT jr.""JobSourceId""
    ,jr.""WeeklyPeriodId""
    ,Sum(jr.""PositionsAvailable"") AS ""Vacancies""
    ,Sum(jr.""JobPostings"") AS ""Postings""
  FROM ""JobStats"" jr
  INNER JOIN ""WeeklyPeriods"" wp ON wp.""Id"" = jr.""WeeklyPeriodId""
  GROUP BY jr.""JobSourceId""
    ,jr.""WeeklyPeriodId""
  )
SELECT d.""Label""
  ,d.""WeekOfMonth""
  ,d.""CalendarMonth""
  ,d.""CalendarYear""
  ,Coalesce(jd.""Vacancies"", 0) AS ""Vacancies""
  ,Coalesce(jd.""Postings"", 0) AS ""Postings""
FROM dimensions d
LEFT OUTER JOIN jobdata jd ON d.""WeeklyPeriodId"" = jd.""WeeklyPeriodId""
  AND d.""JobSourceId"" = jd.""JobSourceId""
ORDER BY d.""Label""
  ,d.""WeekOfMonth""
  ,d.""CalendarMonth""
  ,d.""CalendarYear"";
            ";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobStatsResult>(sql,
                    new
                    {
                        Year = year,
                        Month = month
                    }
                )).ToList();
            }
        }
    }
}