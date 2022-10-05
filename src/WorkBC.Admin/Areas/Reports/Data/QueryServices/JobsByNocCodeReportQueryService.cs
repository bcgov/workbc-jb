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
    ///     Dapper query for the NOC Code Summary Report & Top 20 NOC Codes Report
    /// </summary>
    public class JobsByNocCodeReportQueryService : QueryServiceBase
    {
        internal JobsByNocCodeReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<IList<JobsByNocCodeResult>> RunAsync(DateTime startDate, DateTime endDate, string nocCodeStartsWith = "", int jobSourceId = 0)
        {
            // add 1 day to the endDate so the entire date selected is included in the result
            endDate = endDate.AddDays(1);

            var categoryPredicate = "";

            if (!string.IsNullOrWhiteSpace(nocCodeStartsWith))
            {
                // sanitize for SQL injection (only allow digits)
                var sanitized = new string(nocCodeStartsWith.Where(char.IsDigit).ToArray());
                categoryPredicate = $"WHERE nc.Code LIKE '{sanitized}%'";
            }

            // set up the query
            string sql =
                $@"WITH Jobs(JobId, PositionsAvailable, NocCodeId, JobSourceId) AS
                    (
                        SELECT j.JobId, 
                            j.PositionsAvailable AS Vacancies,
						    j.NocCodeId,
                            j.JobSourceId 
                        FROM dbo.tvf_GetJobsForDate(@EndDate) j
					    WHERE j.DateFirstImported >= @StartDate AND j.DateFirstImported < @EndDate 
                            AND (@JobSourceId = 0 OR j.JobSourceId = @JobSourceId)
                    )

                    SELECT nc.Code AS NocCode, 
                        nc.Title AS NocTitle, 
						IsNull(Sum(j.PositionsAvailable),0) AS Vacancies,
                        Count(j.JobId) AS Postings
					FROM NocCodes nc LEFT OUTER JOIN Jobs j ON j.NocCodeId = nc.Id
                        {categoryPredicate}
                    GROUP BY nc.Code, nc.Title
                    ORDER BY Sum(j.PositionsAvailable) DESC, Count(j.JobId) DESC, nc.Code";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobsByNocCodeResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        JobSourceId = jobSourceId
                    })).ToList();
            }
        }
    }
}