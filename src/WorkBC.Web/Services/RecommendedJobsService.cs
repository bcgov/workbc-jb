using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using Constants = WorkBC.ElasticSearch.Search.Boosts.RecommendedJobsBoost;

namespace WorkBC.Web.Services
{
    public interface IRecommendedJobsService
    {
        Task<RecommendedJobsResultModel> GetRecommendedJobs(RecommendedJobsFilters filters, AccountCriteria accountCriteria);

        Task<long> GetRecommendedJobCount(AccountCriteria accountCriteria);

        Task<AccountCriteria> GetUserAccountCriteria(JobSeeker jobSeeker);
    }

    public class RecommendedJobsService : IRecommendedJobsService
    {
        private const int BritishColumbia = 2;
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _context;
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<RecommendedJobsQuery> _loggerElasticSearch;
        private readonly ViewCountService _viewCountService;

        public RecommendedJobsService(JobBoardContext context, IGeocodingService geocodingService,
            IConfiguration configuration,
            ILogger<RecommendedJobsQuery> loggerElasticSearch, CacheService cacheService)
        {
            _context = context;
            _geocodingService = geocodingService;
            _configuration = configuration;
            _loggerElasticSearch = loggerElasticSearch;
            _viewCountService = new ViewCountService(cacheService, context);
        }

        private RecommendedJobsFilters ElasticFilters { get; set; }
        private AccountCriteria AccountCriteria { get; set; }

        /// <summary>
        ///     Gets recommended jobs for a user
        /// </summary>
        public async Task<RecommendedJobsResultModel> GetRecommendedJobs(RecommendedJobsFilters filters, AccountCriteria accountCriteria)
        {
            // add the job seeker data to the filter that was posted from the ajax call
            BuildFilter(filters, accountCriteria);

            //Get search results from Elastic search
            var esq = new RecommendedJobsQuery(_configuration, ElasticFilters);
            ElasticSearchResponse results = await esq.GetSearchResults();

            //Build the object that we will return to the client
            var model = new RecommendedJobsResultModel
            {
                PageNumber = ElasticFilters.Page,
                PageSize = ElasticFilters.PageSize
            };

            if (results != null)
            {
                if (results.Hits?.HitsHits != null)
                {
                    model.Count = results.Hits.Total.Value ?? 0;

                    // add reasons why the job was recommended
                    IEnumerable<Source> sources = results.Hits.HitsHits.Select(hit => hit.Source);
                    decimal?[] scores = results.Hits.HitsHits.Select(hit => hit.Score).ToArray();

                    model.Result = AddReasons(sources, scores);

                    //Get the number of views for each job and get the data back
                    model.Result = await _viewCountService.GetJobViews(model.Result);
                }
                else
                {
                    model.Result = new Source[0];
                }
            }

            return model;
        }

        /// <summary>
        ///     Gets recommended jobs for a user
        /// </summary>
        public async Task<long> GetRecommendedJobCount(AccountCriteria accountCriteria)
        {
            var filters = new RecommendedJobsFilters
            {
                FilterSavedJobNocs = false,
                FilterSavedJobTitles = false,
                FilterSavedJobEmployers = false,
                FilterIsApprentice = false,
                FilterJobSeekerCity = false,
                FilterIsIndigenous = false,
                FilterIsMatureWorkers = false,
                FilterIsNewcomers = false,
                FilterIsPeopleWithDisabilities = false,
                FilterIsStudents = false,
                FilterIsVeterans = false,
                FilterIsVisibleMinority = false,
                FilterIsYouth = false,
                PageSize = 0,
                Page = 1
            };

            // add the job seeker data to the filter that was posted from the ajax call
            BuildFilter(filters, accountCriteria);

            //Get search results from Elastic search
            var esq = new RecommendedJobsQuery(_configuration, ElasticFilters);
            ElasticSearchResponse results = await esq.GetSearchResults();

            if (results?.Hits?.HitsHits != null)
            {
                return results.Hits.Total.Value ?? 0;
            }

            return 0;
        }

        /// <summary>
        ///     Gets all the raw data needed to do a recommended jobs query in ElasticSearch for a user
        /// </summary>
        public async Task<AccountCriteria> GetUserAccountCriteria(JobSeeker jobSeeker)
        {
            // get the 200 most recent saved jobs for the user
            List<Job> savedJobs = await (from sj in _context.SavedJobs
                                         join j in _context.Jobs on sj.JobId equals j.JobId
                                         where sj.AspNetUserId == jobSeeker.Id && !sj.IsDeleted
                                         orderby sj.Id descending
                                         select new Job
                                         {
                                             JobId = j.JobId,
                                             City = j.City,
                                             DatePosted = j.DatePosted,
                                             //NocCodeId = j.NocCodeId,
                                             NocCodeId2021 = j.NocCodeId2021,
                                             EmployerName = j.EmployerName.ToLower(), // lowercase names for grouping
                                             Title = j.Title.ToLower(), // lowercase names for grouping
                                             IsActive = j.IsActive
                                         }).Take(Constants.MaxSavedJobs).Distinct().ToListAsync();

            // get cityLabel, latitude, longitude
            string cityLabel = await GetUserLocationInfo(jobSeeker);

            // get the user's groups (e.g. Indigenous)
            JobSeekerFlags userFlags =
                await _context.JobSeekerFlags.FirstOrDefaultAsync(f => f.JobSeeker.Id == jobSeeker.Id);

            //// group the saved jobs by noc code
            //Dictionary<short, int> nocCodes = savedJobs
            //    .GroupBy(j => j.NocCodeId ?? 0)
            //    .Select(g => new {Term = g.Key, Count = g.Count()})
            //    .OrderByDescending(a => a.Count)
            //    .ToDictionary(k => k.Term, v => v.Count);

            // group the saved jobs by noc code 2021
            Dictionary<int, int> nocCodes2021 = savedJobs
                .GroupBy(j => j.NocCodeId2021 ?? 0)
                .Select(g => new { Term = g.Key, Count = g.Count() })
                .OrderByDescending(a => a.Count)
                .ToDictionary(k => k.Term, v => v.Count);

            // group the saved jobs by Employer
            Dictionary<string, int> employers = savedJobs
                .GroupBy(j => j.EmployerName)
                .Select(g => new {Term = g.Key, Count = g.Count()})
                .OrderByDescending(a => a.Count)
                .ToDictionary(k => k.Term, v => v.Count);

            // group the saved jobs by title
            Dictionary<string, int> titles = savedJobs
                .GroupBy(j => j.Title)
                .Select(g => new {Term = g.Key, Count = g.Count()})
                .OrderByDescending(a => a.Count)
                .ToDictionary(k => k.Term, v => v.Count);

            // saved job id's should be ignored
            string[] savedJobIds = savedJobs.Where(j => j.IsActive).Select(j => j.JobId.ToString()).ToArray();

            // return all the parameters
            return new AccountCriteria
            {
                InBritishColumbia = jobSeeker.ProvinceId == BritishColumbia,
                City = cityLabel,
                SavedJobIds = savedJobIds,
                UserFlags = userFlags,
                //NocCodes = nocCodes,
                NocCodes2021 = nocCodes2021,
                Employers = employers,
                Titles = titles,
            };
        }

        /// <summary>
        ///     Adds data from the user's account to the filter that was submitted from the
        ///     Angular app.
        /// </summary>
        private void BuildFilter(RecommendedJobsFilters filter, AccountCriteria accountCriteria)
        {
            AccountCriteria = accountCriteria;

            JobSeekerFlags flags = AccountCriteria.UserFlags;

            filter.IsApprentice = flags.IsApprentice;
            filter.IsIndigenous = flags.IsIndigenousPerson;
            filter.IsMatureWorkers = flags.IsMatureWorker;
            filter.IsNewcomers = flags.IsNewImmigrant;
            filter.IsPeopleWithDisabilities = flags.IsPersonWithDisability;
            filter.IsStudents = flags.IsStudent;
            filter.IsVeterans = flags.IsVeteran;
            filter.IsVisibleMinority = flags.IsVisibleMinority;
            filter.IsYouth = flags.IsYouth;

            filter.FilterIsApprentice = filter.FilterIsApprentice && flags.IsApprentice;
            filter.FilterIsIndigenous = filter.FilterIsIndigenous && flags.IsIndigenousPerson;
            filter.FilterIsMatureWorkers = filter.FilterIsMatureWorkers && flags.IsMatureWorker;
            filter.FilterIsNewcomers = filter.FilterIsNewcomers && flags.IsNewImmigrant;
            filter.FilterIsPeopleWithDisabilities = filter.FilterIsPeopleWithDisabilities && flags.IsPersonWithDisability;
            filter.FilterIsStudents = filter.FilterIsStudents && flags.IsStudent;
            filter.FilterIsVeterans = filter.FilterIsVeterans && flags.IsVeteran;
            filter.FilterIsVisibleMinority = filter.FilterIsVisibleMinority && flags.IsVisibleMinority;
            filter.FilterIsYouth = filter.FilterIsYouth && flags.IsYouth;

            if (AccountCriteria.InBritishColumbia)
            {
                filter.City = AccountCriteria.City;
            }

            //filter.NocCodes = AccountCriteria.NocCodes;
            filter.NocCodes2021 = AccountCriteria.NocCodes2021;
            filter.Titles = AccountCriteria.Titles;
            filter.Employers = AccountCriteria.Employers;

            filter.IgnoreJobIdList = AccountCriteria.SavedJobIds;

            ElasticFilters = filter;
        }

        /// <summary>
        ///     Gets the city label for users from BC.  City label is usually just the city name,
        ///     but in cases where there are two places in BC with the same name it also contains
        ///     the region name.
        /// </summary>
        private async Task<string> GetUserLocationInfo(JobSeeker jobSeeker)
        {
            string cityLabel;
            if (jobSeeker.ProvinceId == BritishColumbia)
            {
                cityLabel = (await _context.Locations
                        .FirstOrDefaultAsync(l => l.LocationId == jobSeeker.LocationId))?.Label ?? "_NO_CITY_";
            }
            else
            {
                cityLabel = null;

            }

            return cityLabel;
        }

        /// <summary>
        ///     Adds the Reason to each job (a sentence describing why the job was recommended)
        ///     Also strips out fields that are only needed to construct the reason sentence from
        ///     the final json output.
        /// </summary>
        private Source[] AddReasons(IEnumerable<Source> results, decimal?[] scores)
        {
            var newSource = new List<Source>();

            int i = 0;

            foreach (Source result in results)
            {
                var s = new Source
                {
                    JobId = result.JobId,
                    City = result.City,
                    DatePosted = result.DatePosted,
                    EmployerName = result.EmployerName,
                    ExpireDate = result.ExpireDate,
                    HoursOfWork = result.HoursOfWork,
                    IsFederalJob = result.IsFederalJob,
                    LastUpdated = result.LastUpdated,
                    //Noc = result.Noc,
                    Noc2021 = result.Noc2021,
                    PeriodOfEmployment = result.PeriodOfEmployment,
                    SalarySummary = result.SalarySummary,
                    Title = result.Title,
                    Reason = GetReason(result),
                    ExternalSource = result.ExternalSource,
                    Score = scores[i]
                };

                newSource.Add(s);
                i++;
            }

            return newSource.ToArray();
        }

        /// <summary>
        ///     Gets a sentence describing why a job was recommended
        /// </summary>
        public string GetReason(Source result)
        {
            //return "Recommended ";

            var reasons = new List<string>();

            if (result.Noc2021 != null)
            {                
                int noc= Int32.TryParse(result.Noc2021, out noc) ? noc : 0;
                if (AccountCriteria.NocCodes2021.ContainsKey(noc))
                {
                    int nocCount = AccountCriteria.NocCodes2021[noc];
                    reasons.Add($"based on having the same NOC code as {nocCount.GetWord()} of your saved jobs");
                }
            }

            string title = result.Title.ToLower();
            if (AccountCriteria.Titles.ContainsKey(title))
            {
                int titleCount = AccountCriteria.Titles[title];
                reasons.Add($"based on having the same job title as {titleCount.GetWord()} of your saved jobs");
            }

            string employer = result.EmployerName.ToLower();
            if (AccountCriteria.Employers.ContainsKey(employer))
            {
                int employerCount = AccountCriteria.Employers[employer];
                reasons.Add($"based on having the same employer as {employerCount.GetWord()} of your saved jobs");
            }

            if (!string.IsNullOrWhiteSpace(AccountCriteria?.City) && result.City.Contains(AccountCriteria.City))
            {
                reasons.Add($"based on your location of {AccountCriteria.City} in your account settings");
            }

            var groups = new List<string>();

            if (ElasticFilters.IsNewcomers && (result.IsNewcomer ?? false))
            {
                groups.Add("a newcomer to B.C.");
            }

            if (ElasticFilters.IsStudents && (result.IsStudent ?? false))
            {
                groups.Add("a student");
            }

            if (ElasticFilters.IsVeterans && (result.IsVeteran ?? false))
            {
                groups.Add("a veteran of the Canadian Armed Forces");
            }

            if (ElasticFilters.IsVisibleMinority && (result.IsVismin ?? false))
            {
                groups.Add("a visible minority");
            }

            if (ElasticFilters.IsYouth && (result.IsYouth ?? false))
            {
                groups.Add("a youth");
            }

            if (ElasticFilters.IsPeopleWithDisabilities && (result.IsDisability ?? false))
            {
                groups.Add("a person with a disability");
            }

            if (ElasticFilters.IsIndigenous && (result.IsAboriginal ?? false))
            {
                groups.Add("an Indigenous person");
            }

            if (ElasticFilters.IsApprentice && (result.IsApprentice ?? false))
            {
                groups.Add("an apprentice");
            }

            if (ElasticFilters.IsMatureWorkers && (result.IsMatureWorker ?? false))
            {
                groups.Add("a mature worker");
            }

            var groupsText = "";
            if (groups.Count == 1)
            {
                groupsText = groups[0];
            }
            else if (groups.Count > 1)
            {
                groupsText = $"{string.Join(", ", groups.Take(groups.Count - 1))} or {groups[^1]}";
            }

            if (groups.Any())
            {
                reasons.Add(
                    $"because this job posting encourages applications from those who identify as {groupsText}");
            }

            if (reasons.Count <= 0)
            {
                return "";
            }

            string reason = reasons.Count == 1
                ? $"Recommended {reasons[0]}"
                : $"Recommended {string.Join(", ", reasons.Take(reasons.Count - 1))} and {reasons[^1]}";

            return reason.EndsWith(".") ? reason : $"{reason}.";

        }
    }

    public class AccountCriteria
    {
        public Dictionary<string, int> Titles { get; set; }
        //public Dictionary<short, int> NocCodes { get; set; }
        public Dictionary<int, int> NocCodes2021 { get; set; }
        public Dictionary<string, int> Employers { get; set; }
        public JobSeekerFlags UserFlags { get; set; }
        public string[] SavedJobIds { get; set; }
        public bool InBritishColumbia { get; set; }
        public string City { get; set; }
    }
}