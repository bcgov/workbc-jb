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
    ///     Dapper query for the Job Seeker Detail Report
    /// </summary>
    public class JobSeekerDetailReportQueryService : QueryServiceBase
    {
        internal JobSeekerDetailReportQueryService(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<IList<JobSeekerDetailResult>> RunAsync(DateTime? startDate, DateTime? endDate, string dateRangeType = "any", int maxRows = 1000)
        {
            if (dateRangeType == "any")
            {
                startDate = null;
                endDate = null;
            }
            // add 1 day to the endDate so the entire date selected is included in the result
            if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
            }

            // set up the query
            string sql =
                @"WITH UsersWithJobAlerts AS (
	                SELECT DISTINCT AspNetUserId 
	                FROM JobAlerts 
	                WHERE IsDeleted = 0
                ),
                UsersWithCareerProfiles AS (
	                SELECT DISTINCT AspNetUserId 
	                FROM SavedCareerProfiles 
	                WHERE IsDeleted = 0
                ),
                UsersWithIndustryProfiles AS (
	                SELECT DISTINCT AspNetUserId 
	                FROM SavedIndustryProfiles
	                WHERE IsDeleted = 0
                ),
                UsersWithSavedJobs AS (
	                SELECT DISTINCT AspNetUserId
	                FROM SavedJobs
	                WHERE IsDeleted = 0
                )
                SELECT TOP (@MaxRows) u.Id,
	                u.Email,
	                u.FirstName,
	                u.LastName,
	                u.AccountStatus,
	                l.City,
	                r.[Name] AS Region,
	                (CASE WHEN u.CountryId = 37 AND u.ProvinceId = 2 AND u.LocationId IS NULL THEN NULL ELSE c.[Name] END) AS Country,
	                (CASE WHEN u.CountryId = 37 AND u.ProvinceId = 2 AND u.LocationId IS NULL THEN NULL ELSE p.[Name] END) AS Province,
	                u.DateRegistered,
	                u.LastModified,
	                f.IsApprentice,
	                f.IsIndigenousPerson,
	                f.IsMatureWorker,
	                f.IsNewImmigrant,
	                f.IsPersonWithDisability,
	                f.IsStudent,
	                f.IsVeteran, 
	                f.IsVisibleMinority,
	                f.IsYouth,
	                (CASE WHEN ja.AspNetUserId IS NOT NULL THEN 1 ELSE 0 END) AS HasJobAlerts,
	                (CASE WHEN cp.AspNetUserId IS NOT NULL THEN 1 ELSE 0 END) AS HasSavedCareerProfiles,
	                (CASE WHEN ip.AspNetUserId IS NOT NULL THEN 1 ELSE 0 END) AS HasSavedIndustryProfiles,
	                (CASE WHEN sj.AspNetUserId IS NOT NULL THEN 1 ELSE 0 END) AS HasSavedJobs
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
                WHERE (@StartDate IS NULL OR u.DateRegistered >= @StartDate) 
	                AND (@EndDate IS NULL OR u.DateRegistered <= @EndDate)
                ORDER BY AccountStatus, DateRegistered DESC";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryAsync<JobSeekerDetailResult>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        MaxRows = maxRows
                    })).ToList();
            }
        }

        /// <summary>
        ///     Runs the query
        /// </summary>
        public async Task<int> CountAsync(DateTime? startDate, DateTime? endDate, string dateRangeType = "any")
        {
            if (dateRangeType == "any")
            {
                startDate = null;
                endDate = null;
            }
            // add 1 day to the endDate so the entire date selected is included in the result
            if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
            }

            // set up the query
            string sql =
                @"SELECT COUNT(*)
                FROM AspNetUsers u
                WHERE (@StartDate IS NULL OR u.DateRegistered >= @StartDate) 
	                AND (@EndDate IS NULL OR u.DateRegistered <= @EndDate)";

            using (var conn = new SqlConnection(ConnectionString))
            {
                return (await conn.QueryFirstOrDefaultAsync<int>(sql,
                    new
                    {
                        StartDate = startDate,
                        EndDate = endDate
                    }));
            }
        }
    }
}