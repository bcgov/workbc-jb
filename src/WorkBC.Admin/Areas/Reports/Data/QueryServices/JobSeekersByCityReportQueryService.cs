﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;

namespace WorkBC.Admin.Areas.Reports.Data.QueryServices
{
    /// <summary>
    ///     Dapper query for the Job Seekers by City Report
    /// </summary>
    public class JobSeekersByCityReportQueryService : QueryServiceBase
    {
        internal JobSeekersByCityReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<IList<JobSeekersByCityResult>> RunAsync(DateTime startDate, DateTime endDate,
            int regionId = 0, string dateRangeType = "any")
        {
            // set up the query
            if (dateRangeType != "period")
            {
                var sql = @"SELECT IsNull(l.City,'City not recorded') As City, 
                                    IsNull(r.[Name],'N/A') AS Region, 
                                    Count(*) AS Users  
                            FROM AspNetUsers u
                               LEFT OUTER JOIN Locations l ON l.LocationId = u.LocationId
                               LEFT OUTER JOIN Regions r ON r.Id = l.RegionId
                            WHERE u.AccountStatus <> 99
	                               AND (@RegionId = 0 OR l.RegionId = @RegionId)
                            GROUP BY Isnull(l.City,'City not recorded'), IsNull(r.[Name],'N/A')
                            ORDER BY Count(*) DESC";

                using (var conn = new SqlConnection(ConnectionString))
                {
                    return (await conn.QueryAsync<JobSeekersByCityResult>(sql,
                        new {RegionId = regionId})).ToList();
                }
            }
            else
            {
                // add 1 day to the endDate so the entire date selected is included in the result
                endDate = endDate.AddDays(1);

                var sql = @"SELECT IsNull(l.City,'City not recorded') As City, 
                                    IsNull(r.[Name],'N/A') AS Region, 
                                    Count(*) AS Users  
                            FROM dbo.tvf_GetJobSeekersForDate(@EndDate) js 
	                            LEFT OUTER JOIN Locations l ON l.LocationId = js.LocationId
                                LEFT OUTER JOIN Regions r ON r.Id = l.RegionId
                            WHERE js.DateRegistered >= @StartDate AND js.DateRegistered < @EndDate 
                                    AND (@RegionId = 0 OR l.RegionId = @RegionId)
                            GROUP BY Isnull(l.City,'City not recorded'), IsNull(r.[Name],'N/A')
                            ORDER BY Count(*) DESC";

                using (var conn = new SqlConnection(ConnectionString))
                {
                    return (await conn.QueryAsync<JobSeekersByCityResult>(sql,
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
}