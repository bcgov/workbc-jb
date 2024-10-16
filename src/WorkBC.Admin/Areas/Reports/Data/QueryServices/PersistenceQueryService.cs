using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace WorkBC.Admin.Areas.Reports.Data.QueryServices
{
    /// <summary>
    ///     Dapper queries for running the stored procedures that populate JobStats and JobSeekerStats
    /// </summary>
    public class PersistenceQueryService : QueryServiceBase
    {
        internal PersistenceQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs a stored procedure to populate the JobStats table and the ReportPersistenceControl table
        /// </summary>
        public async Task<int> GenerateJobStats(DateTime weekEndDate)
        {
            try
            {
                return await ExecuteProcedure("usp_GenerateJobStats", weekEndDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"GenerateJobStats failed. weekEndDate={weekEndDate:yyyy-MMM-dd}", ex);
            }
        }

        /// <summary>
        ///     Runs a stored procedure to populate the JobSeekerStats table and the ReportPersistenceControl table
        /// </summary>
        public async Task<int> GenerateJobSeekerStats(DateTime weekEndDate)
        {
            try
            {
                return await ExecuteProcedure("usp_GenerateJobSeekerStats", weekEndDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"GenerateJobSeekerStats failed. weekEndDate={weekEndDate:yyyy-MMM-dd}", ex);
            }
        }

        /// <summary>
        ///     Executes the stored procedures
        /// </summary>
        private async Task<int> ExecuteProcedure(string procedureName, DateTime weekEndDate)
        {
            var queryParams = new DynamicParameters();
            queryParams.Add("@WeekEndDate", dbType: DbType.DateTime, value: weekEndDate);
            queryParams.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.ExecuteAsync(procedureName, queryParams,
                    commandType: CommandType.StoredProcedure);

                var returnVal = queryParams.Get<int>("@RetVal");

                if (returnVal == -1)
                {
                    throw new DataException(
                        "Internal TRY/CATCH in stored procedure detected an error.  Changes were rolled back.");
                }

                return 0;
            }
        }
    }
}