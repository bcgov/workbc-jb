using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBC.Admin.Areas.Jobs.Models;
using WorkBC.Admin.Areas.Jobs.Services;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Admin.Areas.Jobs.Controllers
{
    [Authorize(Roles = MinAccessLevel.Admin)]
    [Area("Jobs")]
    [Route("jobs/[controller]/[action]")]
    public class JobManagementController : AdminControllerBase
    {
        private readonly JobBoardContext _jobBoardContext;
        private readonly IJobService _service;

        public JobManagementController(JobBoardContext jobBoardContext, IJobService service)
        {
            _jobBoardContext = jobBoardContext;
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(string jobId)
        {
            await _service.DeleteJob(Convert.ToInt64(jobId), base.CurrentAdminUserId);

            return RedirectToAction("Index", "JobSearch");
        }


        public async Task<IActionResult> ViewJobPostingHistory(string id)
        {
            if (id == null)
            {
                return BadRequest("id is required");
            }

            JobPostingHistoryViewModel model = await GetJobPostingHistory(Convert.ToInt64(id));

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        private async Task<JobPostingHistoryViewModel> GetJobPostingHistory(long id)
        {
            var model = new JobPostingHistoryViewModel
            {
                History = new List<JobPostingHistoryItem>()
            };

            // get the JobId record which contains the DateFirstImported
            JobId jobId = await _jobBoardContext.JobIds.FirstOrDefaultAsync(j => j.Id == id);

            if (jobId == null)
            {
                return null;
            }

            string importer = jobId.JobSourceId == JobSource.Wanted
                ? "WorkBC.Importers.Wanted"
                : "WorkBC.Importers.Federal";

            Job job = await _jobBoardContext.Jobs.FirstOrDefaultAsync(j => j.JobId == id);

            model.City = job.City;
            model.JobTitle = job.Title;
            model.JobId = job.JobId;

            model.History.Add(new JobPostingHistoryItem
            {
                Activity = "Job created",
                Editor = importer,
                TimeStamp = jobId.DateFirstImported,
                OldValue = "",
                NewValue = ""
            });

            List<JobVersion> jobVersions = await _jobBoardContext.JobVersions
                .Where(jv => jv.JobId == id)
                .OrderBy(jv => jv.VersionNumber).ToListAsync();

            if (jobVersions.Count() > 1)
            {
                for (var i = 0; i < jobVersions.Count() - 1; i++)
                {
                    JobVersion oldVersion = jobVersions[i];
                    JobVersion newVersion = jobVersions[i + 1];

                    if (oldVersion.IsActive != newVersion.IsActive)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "Job status updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = oldVersion.IsActive ? "Active" : "Inactive",
                            NewValue = newVersion.IsActive ? "Active" : "Inactive"
                        });
                    }

                    if (oldVersion.DatePosted != newVersion.DatePosted)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "Date refreshed updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = $"{oldVersion.DatePosted:yyyy-MM-dd HH:mm} PST",
                            NewValue = $"{newVersion.DatePosted:yyyy-MM-dd HH:mm} PST"
                        });
                    }

                    if (oldVersion.IndustryId != newVersion.IndustryId)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "Industry id updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = oldVersion.IndustryId.ToString(),
                            NewValue = newVersion.IndustryId.ToString()
                        });
                    }

                    if (oldVersion.NocCodeId2021 != newVersion.NocCodeId2021)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "NOC code updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = (oldVersion.NocCodeId2021 ?? 0).ToString("00000"),
                            NewValue = (newVersion.NocCodeId2021 ?? 0).ToString("00000")
                        });
                    }


                    if (oldVersion.LocationId != newVersion.LocationId)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "City updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = (await _jobBoardContext.Locations.FirstOrDefaultAsync(l =>
                                l.LocationId == oldVersion.LocationId))?.City ?? "",
                            NewValue = (await _jobBoardContext.Locations.FirstOrDefaultAsync(l =>
                                l.LocationId == newVersion.LocationId))?.City ?? ""
                        });
                    }

                    if (oldVersion.PositionsAvailable != newVersion.PositionsAvailable)
                    {
                        model.History.Add(new JobPostingHistoryItem
                        {
                            Activity = "Positions available updated",
                            Editor = importer,
                            TimeStamp = newVersion.DateVersionStart,
                            OldValue = oldVersion.PositionsAvailable.ToString(),
                            NewValue = newVersion.PositionsAvailable.ToString()
                        });
                    }
                }
            }

            DeletedJob deletedJob = await _jobBoardContext.DeletedJobs.FirstOrDefaultAsync(j => j.JobId == id);

            if (deletedJob != null)
            {
                model.History.Add(new JobPostingHistoryItem
                {
                    Activity = "Job was deleted",
                    Editor = deletedJob.DeletedByAdminUser.DisplayName,
                    TimeStamp = deletedJob.DateDeleted,
                    OldValue = "",
                    NewValue = ""
                });
            }

            return model;
        }
    }
}
