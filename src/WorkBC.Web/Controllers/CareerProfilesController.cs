using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;
using WorkBC.Web.Models;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [Route("api/career-profiles")]
    [ApiController]
    public class CareerProfilesController : ControllerBase
    {
        private readonly JobBoardContext _context;
        private readonly IConfiguration _configuration;
        private readonly IGeocodingService _geocodingService;

        public CareerProfilesController(JobBoardContext context,
            IConfiguration configuration, IGeocodingService geocodingService)
        {
            _context = context;
            _configuration = configuration;
            _geocodingService = geocodingService;
        }

        // GET: api/career-profiles
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<ActionResult<IEnumerable<CareerProfileModel>>> GetSavedCareerProfiles()
        {
            var savedCareerProfiles = await _context.SavedCareerProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted && x.NocCodeId2021 != null)
                .ToListAsync();

            var savedCareerProfilesDict = savedCareerProfiles
                .GroupBy(s => s.Id)
                .ToDictionary(
                    s => s.Key,
                    s => s.First().Id
                );

            List<int> savedCareerProfilesIds = savedCareerProfilesDict.Select(s => s.Key).ToList();

            IQueryable<CareerProfileModel> query = from p in _context.SavedCareerProfiles
                join n in _context.NocCodes2021 on p.NocCodeId2021 equals n.Id
                orderby n.Title
                where savedCareerProfilesIds.Contains(p.Id)
                select new CareerProfileModel
                {
                    Id = savedCareerProfilesDict[p.Id],
                    Title = n.Title,
                    NocCode = n.Code
                };

            return Ok(await query.ToListAsync());
        }

        [HttpGet("total")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<IActionResult> GetCareerProfilesTotalAsync()
        {
            var savedCareerProfiles = await _context.SavedCareerProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted && x.NocCodeId2021 != null)
                .ToListAsync();

            return Ok(savedCareerProfiles.Count);
        }

        // GET: api/career-profiles/6
        [HttpGet("{id}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<ActionResult<SavedCareerProfile>> GetSavedCareerProfile(int id)
            {
            var savedCareerProfile = await _context.SavedCareerProfiles.FindAsync(id);

            if (savedCareerProfile == null)
            {
                return NotFound();
            }

            return savedCareerProfile;
        }

        // PUT: api/career-profiles/6
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSavedCareerProfile(int id, SavedCareerProfile savedCareerProfile)
        {
            if (id != savedCareerProfile.Id)
            {
                return BadRequest();
            }

            _context.Entry(savedCareerProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SavedCareerProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/career-profiles
        [HttpPost]
        public async Task<ActionResult<SavedCareerProfile>> PostSavedCareerProfile(SavedCareerProfile savedCareerProfile)
        {
            _context.SavedCareerProfiles.Add(savedCareerProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSavedCareerProfile", new { id = savedCareerProfile.Id }, savedCareerProfile);
        }

        // DELETE: api/career-profiles/6
        [HttpDelete("{id}")]
        public async Task<ActionResult<SavedCareerProfile>> DeleteSavedCareerProfile(int id)
        {
            var savedCareerProfile = await _context.SavedCareerProfiles.FindAsync(id);
            if (savedCareerProfile == null)
            {
                return NotFound();
            }

            savedCareerProfile.IsDeleted = true;
            savedCareerProfile.DateDeleted = DateTime.Now;
            //_context.SavedCareerProfiles.Remove(savedCareerProfile);
            await _context.SaveChangesAsync();

            return savedCareerProfile;
        }

        // POST: api/career-profiles/Save/11101
        [HttpPost("save/{noc}")]
        public async Task<bool> SaveCareerProfile(string noc)
        {
            var nocList = noc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string nocCode in nocList)
            {
                //Find the saved career profile id based on the noc code 2021 which is 5 digits long.
                int careerProfileId = (
                    from profile in _context.SavedCareerProfiles
                    join nocCodes in _context.NocCodes2021 on profile.NocCodeId2021 equals nocCodes.Id
                    where nocCodes.Code == nocCode && profile.AspNetUserId == UserId && !profile.IsDeleted
                    select profile.Id
                ).FirstOrDefault();

                if (careerProfileId == 0)
                {
                        //only add career profile if it does not exist for this job-seeker
                        var profile = new SavedCareerProfile
                        {
                            AspNetUserId = UserId,
                            CareerProfileId = null,
                            NocCodeId2021 = Convert.ToInt32(nocCode),
                            DateDeleted = null,
                            DateSaved = DateTime.Now,
                            IsDeleted = false
                        };

                        _context.SavedCareerProfiles.Add(profile);
                        await _context.SaveChangesAsync();

                }
            }

            return true;
        }

        // GET: api/career-profiles/Status/11101
        [HttpGet("status/{noc}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<bool> GetCareerProfileStatus(int noc)
        {
                SavedCareerProfile savedCareerProfile = await _context.SavedCareerProfiles
                    .FirstOrDefaultAsync(x =>
                        x.AspNetUserId == UserId &&
                        x.NocCodeId2021 == noc &&
                        !x.IsDeleted);

                if (savedCareerProfile != null)
                {
                    //this career profile is linked to this user
                    return true;
                }

            return false;
        }

        [HttpGet("topjobs/{noc}")]
        [AllowAnonymous]
        public async Task<List<CareerProfileTopJobsModel>> GetTopJobsByNoc(int noc)
        {
            // Create a search filter to get the newest 4 jobs for a specific NOC code.
            // (we will only display 3 jobs, but if there's 4 jobs available we know to display
            // additional text in Kentico)
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 4,
                SortOrder = 1, // Posted date newest first
                SearchNocField = noc.ToString(),
            };

            // run the query
            Source[] jobs = await RunElasticQuery(filters);

            if (!jobs.Any())
            {
                return new List<CareerProfileTopJobsModel>();
            }

            // return the list of jobs
            return jobs.Select(
                job => new CareerProfileTopJobsModel
                {
                    JobId = job.JobId,
                    DatePosted = job.DatePosted.Value.ToString("MMMM dd, yyyy"),
                    Employer = job.EmployerName,
                    JobTitle = job.Title,
                    Location = string.Join(", ", job.City),
                    ExternalUrl = job.ExternalSource?.Source?.FirstOrDefault()?.Url ?? "",
                    ExteralSource = job.ExternalSource?.Source?.FirstOrDefault()?.Source ?? ""
                }
            ).ToList();
        }


        [HttpGet("topjobs/{noc}")]
        [AllowAnonymous]
        public async Task<List<CareerProfileTopJobsModel>> GetTopJobsByNoc2021(int noc2021)
        {
            // Create a search filter to get the newest 4 jobs for a specific NOC code.
            // (we will only display 3 jobs, but if there's 4 jobs available we know to display
            // additional text in Kentico)
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 4,
                SortOrder = 1, // Posted date newest first
                SearchNocField = noc2021.ToString(),
            };

            // run the query
            Source[] jobs = await RunElasticQuery(filters);

            if (!jobs.Any())
            {
                return new List<CareerProfileTopJobsModel>();
            }

            // return the list of jobs
            return jobs.Select(
                job => new CareerProfileTopJobsModel
                {
                    JobId = job.JobId,
                    DatePosted = job.DatePosted.Value.ToString("MMMM dd, yyyy"),
                    Employer = job.EmployerName,
                    JobTitle = job.Title,
                    Location = string.Join(", ", job.City),
                    ExternalUrl = job.ExternalSource?.Source?.FirstOrDefault()?.Url ?? "",
                    ExteralSource = job.ExternalSource?.Source?.FirstOrDefault()?.Source ?? ""
                }
            ).ToList();
        }

        private async Task<Source[]> RunElasticQuery(JobSearchFilters filters)
        {
            //Search object that we will use to search Elastic Search
            var query = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results from Elastic search
            ElasticSearchResponse results = await query.GetSearchResults();

            return results?.Hits?.HitsHits != null
                ? results.Hits.HitsHits.Select(hit => hit.Source).ToArray()
                : new Source[] { };
        }

        private bool SavedCareerProfileExists(int id)
        {
            return _context.SavedCareerProfiles.Any(e => e.Id == id);
        }

        private string UserId => HttpContext.User.Identity.Name;
    }
}
