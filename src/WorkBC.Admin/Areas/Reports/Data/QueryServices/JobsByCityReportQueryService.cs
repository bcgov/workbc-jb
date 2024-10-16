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
    ///     Dapper query for the Jobs by City Report
    /// </summary>
    public class JobsByCityReportQueryService : QueryServiceBase
    {
        internal JobsByCityReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<IList<JobsByCityResult>> RunAsync(DateTime startDate, DateTime endDate, int regionId = 0, int jobSourceId = 0)
        {
            // add 1 day to the endDate so the entire date selected is included in the result
            endDate = endDate.AddDays(1);

            // set up the query
            string sql = @"
SELECT l.""City""
	,Coalesce(r.""Name"", 'N/A') AS ""Region""
	,Sum(j.""PositionsAvailable"") AS ""Vacancies""
	,Count(*) AS ""Postings""
FROM ""tvf_GetJobsForDate""(@EndDate) j
INNER JOIN ""Locations"" l ON l.""LocationId"" = j.""LocationId""
INNER JOIN ""Regions"" r ON r.""Id"" = l.""RegionId""
WHERE j.""DateFirstImported"" >= @StartDate
	AND j.""DateFirstImported"" < @EndDate
	AND (
		@RegionId = 0
		OR l.""RegionId"" = @RegionId
		)
	AND (
		@JobSourceId = 0
		OR j.""JobSourceId"" = @JobSourceId
		)
GROUP BY l.""City""
	,Coalesce(r.""Name"", 'N/A')
ORDER BY Sum(j.""PositionsAvailable"") DESC;
";

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobsByCityResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        RegionId = regionId,
                        JobSourceId = jobSourceId
                    })).ToList();
            }
        }
    }
}