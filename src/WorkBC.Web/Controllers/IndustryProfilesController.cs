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
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Web.Models;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [Route("api/industry-profiles")]
    [ApiController]
    public class IndustryProfilesController : ControllerBase
    {
        private readonly JobBoardContext _context;
        private SsotApi _ssotApi;
        private readonly IConfiguration _configuration;

        public IndustryProfilesController(JobBoardContext context, SsotApi ssotApi, IConfiguration configurtion)
        {
            _context = context;
            _ssotApi = ssotApi;
            _configuration = configurtion;
        }

        [HttpGet("total")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<IActionResult> GetIndustryProfilesTotalAsync()
        {
            var savedIndustryProfiles = await _context.SavedIndustryProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted)
                .ToListAsync();

            return Ok(savedIndustryProfiles.Count);
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<ActionResult<IEnumerable<IndustryProfileModel>>> GetSavedIndustryProfilesAsync()
        {
            var savedIndustryProfiles = await _context.SavedIndustryProfiles
                .Where(x => x.AspNetUserId == UserId && !x.IsDeleted)
                .ToListAsync();

            var savedIndustryProfilesDict = savedIndustryProfiles
                .GroupBy(s => s.IndustryProfileId)
                .ToDictionary(
                    s => s.Key,
                    s => s.First().Id
                );

            // todo: don't delete this until we confirm that we are not adding job search counts and links to saved industry profiles
            // get job counts and industry Id mappings 
            //Dictionary<int, IndustryProfileModel> jobSearchInfo = savedIndustryProfiles.Any()
            //    ? await GetIndustryIdsAndJobCounts()
            //    : new Dictionary<int, IndustryProfileModel>();

            var industryProfiles = await _ssotApi.GetIndustryProfiles(savedIndustryProfilesDict.Keys.ToList());
            
            var results = new List<IndustryProfileModel>();
            
            if (industryProfiles.Count > 0)
            {
                foreach (var industryProfile in industryProfiles)
                {
                    results.Add(new IndustryProfileModel()
                    {
                        Id = savedIndustryProfilesDict[industryProfile.IndustryProfileId],
                        Title = industryProfile.PageTitle
                    });
                }
            }
            
            return Ok(results);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSavedIndustryProfileAsync(int id)
        {
            var savedIndustryProfile = await _context.SavedIndustryProfiles.FindAsync(id);
            if (savedIndustryProfile == null || savedIndustryProfile.IsDeleted)
            {
                return NotFound();
            }

            savedIndustryProfile.IsDeleted = true;
            savedIndustryProfile.DateDeleted = DateTime.Now;
            return Ok(await _context.SaveChangesAsync());
        }

        // POST: api/industry-profiles/Save/1
        [HttpPost("save/{naicsId}")]
        public async Task<bool> SaveIndustryProfile(string naicsId)
        {
            var lstNaics = naicsId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string naics in lstNaics)
            {
                var industryProfileId = await _ssotApi.GetIndustryProfileIdByNaics(naics);

                //find the industry profile id based on the naics code
                SavedIndustryProfile savedIndustryProfile = _context.SavedIndustryProfiles
                    .FirstOrDefault(x =>
                        x.AspNetUserId == UserId &&
                        x.IndustryProfileId == industryProfileId &&
                        !x.IsDeleted);

                if (savedIndustryProfile == null && industryProfileId > 0)
                {
                    //only add industry profile if it does not exist for this job-seeker
                    var profile = new SavedIndustryProfile
                    {
                        AspNetUserId = UserId,
                        IndustryProfileId = industryProfileId,
                        DateDeleted = null,
                        DateSaved = DateTime.Now,
                        IsDeleted = false
                    };

                    _context.SavedIndustryProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        // GET: api/industry-profile/Status/1
        [HttpGet("status/{naicsId}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<bool> GetIndustryProfileStatus(int naicsId)
        {
            //find the industry profile id based on the naics code
            var industryProfileId = await _ssotApi.GetIndustryProfileIdByNaics(naicsId.ToString("0000")); 

            if (industryProfileId > 0)
            {
                SavedIndustryProfile savedIndustryProfile = await _context.SavedIndustryProfiles
                    .FirstOrDefaultAsync(x =>
                        x.AspNetUserId == UserId &&
                        x.IndustryProfileId == industryProfileId &&
                        !x.IsDeleted);

                if (savedIndustryProfile != null)
                {
                    //this industry profile is linked to this user
                    return true;
                }
            }

            return false;
        }

        // todo: don't delete this until we confirm that we are not adding job search counts and links to saved industry profiles
        // /// <summary>
        // ///     Returns a dictionary with the information needed to display job counts and links to search results
        // ///     for each industry profile
        // /// </summary>
        //private async Task<Dictionary<int, IndustryProfileModel>> GetIndustryIdsAndJobCounts()
        //{
        //    // get counts from Elasticsearch
        //    dynamic esr = await new IndustryAggregationQuery().GetResultsFromElasticSearch(_configuration);

        //    // create a dictionary from the Elasticsearch results
        //    var counts = new Dictionary<int, int>();
        //    foreach (dynamic industry in esr.aggregations.industries.buckets)
        //    {
        //        counts.Add((int)industry.key, (int)industry.doc_count);
        //    }

        //    // get the Naics to IndustryId mappings
        //    List<IndustryNaics> naicsMapping = await _context.IndustryNaics.ToListAsync();

        //    // combine the Naics to IndustryId mappings with the Elasticsearch results
        //    return naicsMapping.GroupBy(n => n.NaicsId)
        //        .ToDictionary(
        //            g => (int)g.Key,
        //            i => new IndustryProfileModel
        //            {
        //                IndustryIds = string.Join(",", i.Select(h => h.IndustryId)),
        //                Count = i.Sum(h => counts.ContainsKey(h.IndustryId) ? counts[h.IndustryId] : 0)
        //            }
        //        );
        //}

        private string UserId => HttpContext.User.Identity.Name;
    }
}
