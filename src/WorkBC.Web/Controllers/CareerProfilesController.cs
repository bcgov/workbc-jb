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
        private readonly SsotContext _enterpriseContext;
        private readonly IConfiguration _configuration;
        private readonly IGeocodingService _geocodingService;

        public CareerProfilesController(JobBoardContext context, SsotContext enterpriseContext,
            IConfiguration configuration, IGeocodingService geocodingService)
        {
            _context = context;
            _enterpriseContext = enterpriseContext;
            _configuration = configuration;
            _geocodingService = geocodingService;
        }

        // GET: api/career-profiles
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<ActionResult<IEnumerable<CareerProfileModel>>> GetSavedCareerProfiles()
        {
            var savedCareerProfiles = await _context.SavedCareerProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted)
                .ToListAsync();

            var savedCareerProfilesDict = savedCareerProfiles
                .GroupBy(s => s.CareerProfileId)
                .ToDictionary(
                    s => s.Key,
                    s => s.First().Id
                );

            List<int> savedCareerProfilesIds = savedCareerProfilesDict.Select(s => s.Key).ToList();

            IQueryable<CareerProfileModel> query = from p in _enterpriseContext.CareerProfiles
                join n in _enterpriseContext.Nocs on p.NocId equals n.NocId
                orderby n.NameEnglish
                where savedCareerProfilesIds.Contains(p.CareerProfileId)
                select new CareerProfileModel
                {
                    Id = savedCareerProfilesDict[p.CareerProfileId],
                    Title = n.NameEnglish,
                    NocCode = n.Noccode
                };

            return Ok(await query.ToListAsync());
        }

        [HttpGet("total")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<IActionResult> GetCareerProfilesTotalAsync()
        {
            var savedCareerProfiles = await _context.SavedCareerProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted)
                .ToListAsync();

            return Ok(savedCareerProfiles.Count);
        }

        // GET: api/CareerProfiles/5
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

        // PUT: api/CareerProfiles/5
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

        // POST: api/CareerProfiles
        [HttpPost]
        public async Task<ActionResult<SavedCareerProfile>> PostSavedCareerProfile(SavedCareerProfile savedCareerProfile)
        {
            _context.SavedCareerProfiles.Add(savedCareerProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSavedCareerProfile", new { id = savedCareerProfile.Id }, savedCareerProfile);
        }

        // DELETE: api/CareerProfiles/5
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

        // POST: api/CareerProfiles/Save/1234
        [HttpPost("save/{noc}")]
        public async Task<bool> SaveCareerProfile(string noc)
        {
            var nocList = noc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string nocCode in nocList)
            {
                //find the career profile id based on the noc code
                int careerProfileId = (
                    from profile in _enterpriseContext.CareerProfiles
                    join nocCodes in _enterpriseContext.Nocs on profile.NocId equals nocCodes.NocId
                    where nocCodes.Nocyear == 2011 && nocCodes.Noccode == nocCode.PadLeft(4,'0')
                    select profile.CareerProfileId
                ).FirstOrDefault();

                if (careerProfileId > 0)
                {
                    SavedCareerProfile savedCareerProfile = _context.SavedCareerProfiles
                        .FirstOrDefault(x =>
                            x.AspNetUserId == UserId &&
                            x.CareerProfileId == careerProfileId &&
                            !x.IsDeleted);

                    if (savedCareerProfile == null)
                    {
                        //only add career profile if it does not exist for this job-seeker
                        var profile = new SavedCareerProfile
                        {
                            AspNetUserId = UserId,
                            CareerProfileId = careerProfileId,
                            DateDeleted = null,
                            DateSaved = DateTime.Now,
                            IsDeleted = false
                        };

                        _context.SavedCareerProfiles.Add(profile);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return true;
        }

        // GET: api/CareerProfiles/Status/1234
        [HttpGet("status/{noc}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<bool> GetCareerProfileStatus(int noc)
        {
            //find the career profile id based on the noc code
            int careerProfileId = (from profile in _enterpriseContext.CareerProfiles
                join nocCodes in _enterpriseContext.Nocs on profile.NocId equals nocCodes.NocId
                where nocCodes.Nocyear == 2011 && nocCodes.Noccode == noc.ToString("0000")
                select profile.CareerProfileId).FirstOrDefault();

            if (careerProfileId > 0)
            {
                SavedCareerProfile savedCareerProfile = await _context.SavedCareerProfiles
                    .FirstOrDefaultAsync(x =>
                        x.AspNetUserId == UserId &&
                        x.CareerProfileId == careerProfileId &&
                        !x.IsDeleted);

                if (savedCareerProfile != null)
                {
                    //this career profile is linked to this user
                    return true;
                }
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
