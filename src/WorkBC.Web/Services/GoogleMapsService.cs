using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Web.Models.Search;

namespace WorkBC.Web.Services
{
    public class GoogleMapsService
    {
        public async Task<List<GooglesMapsInfoWindowDetail>> GetJobsForLocation(string jobIds, IConfiguration configuration)
        {
            List<long> ids = null;
            List<GooglesMapsInfoWindowDetail> results = new List<GooglesMapsInfoWindowDetail>();
            ElasticSearchResponse esr;

            //get results from ES
            if (jobIds.Length > 10000)
            {
                // if the list of jobids is more than 10000 characters then query based on latitude and longitude
                ids = ParseJobIds(jobIds);

                Location[] location;
                var i = 0;
                do
                {
                    location = await GetGeoLocation(ids[i], configuration);
                    i++;
                } while (i < ids.Count && location.Length > 1);

                esr = await new GoogleMapsInfoWindowQuery().GetResultsFromElasticSearch(configuration, jobIds,
                    location[0]);
            } 
            else
            {
                // if the list of jobids is 10000 or fewer characters then query based on the list of jobsIds
                esr = await new GoogleMapsInfoWindowQuery().GetResultsFromElasticSearch(configuration, jobIds);
            }

            //Process in the correct format
            Source[] jobs = esr.Hits.HitsHits.Select(hit => hit.Source).ToArray();

            foreach(Source job in jobs)
            {
                if (ids == null || ids.Contains(job.JobId))
                {
                    GooglesMapsInfoWindowDetail jobData = new GooglesMapsInfoWindowDetail()
                    {
                        City = string.Join(", ", job.City),
                        DatePosted = job.DatePosted.HasValue ? job.DatePosted.Value.ToString() : string.Empty,
                        Expire = job.ExpireDate.HasValue ? job.ExpireDate.Value.ToString() : string.Empty,
                        HoursOfWork = string.Join(", ", (job.HoursOfWork != null ? job.HoursOfWork.Description : new List<string>())),
                        JobTitle = job.Title,
                        PeriodOfEmployment = string.Join(", ", (job.PeriodOfEmployment != null ? job.PeriodOfEmployment.Description : new List<string>())),
                        Salary = job.SalarySummary,
                        Company = job.EmployerName,
                        JobId = job.JobId,
                        IsFederalJob = job.IsFederalJob.HasValue ? job.IsFederalJob.Value : false,
                        JobSource = job.ExternalSource != null && job.ExternalSource.Source != null && job.ExternalSource.Source.Count > 0 ? job.ExternalSource.Source[0].Source : string.Empty,
                        ExternalUrl = job.ExternalSource != null && job.ExternalSource.Source != null && job.ExternalSource.Source.Count > 0 ? job.ExternalSource.Source[0].Url : string.Empty
                    };

                    results.Add(jobData);
                }
            }

            return results;
        }

        public async Task<Location[]> GetGeoLocation(long jobId, IConfiguration configuration)
        {
            if (jobId > 0)
            {
                Source[] job = (await new JobDetailQuery(configuration).GetSearchResults(jobId, "en"))?.Hits?.HitsHits?.Select(hit => hit.Source)?.ToArray();

                if (job?.Length > 0)
                {
                    return job[0].Location;
                }
            }

            return null;
        }

        public List<long> ParseJobIds(string jobIds)
        {
            var ids = new List<long>();

            foreach (string j in jobIds.Split(","))
            {
                long.TryParse(j, out long v);
                if (v > 0)
                {
                    ids.Add(v);
                }
            }

            return ids;
        }
    }
}
