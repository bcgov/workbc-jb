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
    ///     Dapper query for the Jobs by Industry Report
    /// </summary>
    public class JobsByIndustryReportQueryService : QueryServiceBase
    {
        internal JobsByIndustryReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<IList<JobsByIndustryResult>> RunAsync(DateTime startDate, DateTime endDate, int regionId = 0)
        {
            // add 1 day to the endDate so the entire date selected is included in the result
            endDate = endDate.AddDays(1);

            // set up the query
            string sql =
                @"WITH JobData AS (
                        SELECT j.IndustryId,
                            Sum(j.PositionsAvailable) AS Vacancies,
                            Count(*) AS Postings
                        FROM dbo.tvf_GetJobsForDate(@EndDate) j
						    INNER JOIN Locations ll ON ll.LocationId = j.LocationId
                        WHERE j.DateFirstImported >= @StartDate AND j.DateFirstImported < @EndDate
                                AND (@RegionId = 0 OR ll.RegionId = @RegionId)
							    AND j.JobSourceId = 1
                        GROUP BY j.IndustryId
                )
                SELECT i.Id, i.Title AS Industry,
                    ISNULL(Vacancies,0) AS Vacancies,
                    ISNULL(Postings,0) AS Postings
                FROM Industries i
                LEFT OUTER JOIN JobData j ON j.IndustryId = i.Id
                WHERE i.Id NOT IN (38, 33)
                ORDER BY Vacancies DESC, Postings DESC, Title";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobsByIndustryResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        RegionId = regionId
                    })).ToList();
            }
        }
    }
}