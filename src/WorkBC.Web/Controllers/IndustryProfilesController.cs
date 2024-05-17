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
using WorkBC.Web.Models;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [Route("api/industry-profiles")]
    [ApiController]
    public class IndustryProfilesController : ControllerBase
    {
        private readonly JobBoardContext _context;
        private readonly IConfiguration _configuration;

        public IndustryProfilesController(JobBoardContext context, IConfiguration configurtion)
        {
            _context = context;
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
                .GroupBy(s => s.IndustryId)
                .ToDictionary(
                    s => s.Key,
                    s => s.First().Id
                );

        //The title published on the UI is the BC mandated one.
        var query = from p in _context.Industries
                        where savedIndustryProfilesDict.Keys.Contains(p.Id)
                        orderby p.Title
                        select new IndustryProfileModel
                        {
                            Id = savedIndustryProfilesDict[p.Id],
                            Title = p.TitleBC
                        };

            return Ok(await query.ToListAsync());
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

        //Save the IndustryProfile based on the IndustryId in JobBoard database.
        // POST: api/industry-profiles/Save/1
        [HttpPost("save/{industryId}")]
        public async Task<bool> SaveIndustryProfile(string industryId)
        {
            var lstIds = industryId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in lstIds)
            {
                //find if the industry id passed is valid.
                int validId = (from p in _context.Industries
                                  where p.Id == Convert.ToInt16(id)
                                  orderby p.Title
                                  select p.Id).FirstOrDefault();

                //find the industry profile based on the industry id for this user.
                SavedIndustryProfile savedIndustryProfile = _context.SavedIndustryProfiles
                    .FirstOrDefault(x =>
                        x.AspNetUserId == UserId &&
                        x.IndustryId == Convert.ToInt16(id) &&
                        !x.IsDeleted);

                if (validId > 0 && savedIndustryProfile == null)
                {
                    //only add industry profile if it does not exist for this job-seeker
                    var profile = new SavedIndustryProfile
                    {
                        AspNetUserId = UserId,
                        IndustryId = Convert.ToInt16(id),
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
        [HttpGet("status/{industryId}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<bool> GetIndustryProfileStatus(int industryId)
        {
            if (industryId > 0)
            {
                SavedIndustryProfile savedIndustryProfile = await _context.SavedIndustryProfiles
                    .FirstOrDefaultAsync(x =>
                        x.AspNetUserId == UserId &&
                        x.IndustryId == industryId &&
                        !x.IsDeleted);

                if (savedIndustryProfile != null)
                {
                    //this industry profile is linked to this user
                    return true;
                }
            }

            return false;
        }

        private string UserId => HttpContext.User.Identity.Name;
    }
}
