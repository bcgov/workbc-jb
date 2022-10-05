using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Web.Models;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Web.Services
{
    public interface ISavedJobsService
    {
        Task<int> SaveJobsAsync(string userId, long[] jobIds);
        Task DeleteSavedJobAsync(string userId, string jobId);
        Task<IList<long>> GetSavedJobIdsAsync(string userId);
        Task<IList<SavedJobsModel>> GetSavedJobsAsync(string userId);
        Task SaveJobNoteAsync(JobNoteModel jobNoteModel);
    }

    public class SavedJobsService : ISavedJobsService
    {
        private readonly JobBoardContext _context;

        public SavedJobsService(JobBoardContext context)
        {
            _context = context;
        }

        public async Task<int> SaveJobsAsync(string userId, long[] jobIds)
        {
            var jobsInserted = 0;

            foreach (long jobId in jobIds)
            {
                Job job = await _context.Jobs.FirstOrDefaultAsync(x => x.JobId == jobId && x.IsActive);
                if (job != null)
                {
                    SavedJob savedJob = await _context.SavedJobs.FirstOrDefaultAsync(x =>
                        x.JobId == jobId && x.AspNetUserId == userId && !x.IsDeleted);

                    if (savedJob == null)
                    {
                        savedJob = new SavedJob
                        {
                            AspNetUserId = userId,
                            JobId = jobId,
                            DateSaved = DateTime.Now
                        };

                        await _context.SavedJobs.AddAsync(savedJob);
                        await _context.SaveChangesAsync();

                        jobsInserted++;
                    }
                }
            }

            return jobsInserted;
        }

        public async Task<IList<long>> GetSavedJobIdsAsync(string userId)
        {
            List<long> result = await _context.SavedJobs
                .Where(x => x.AspNetUserId == userId && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => x.JobId)
                .ToListAsync();

            return result;
        }

        public async Task DeleteSavedJobAsync(string userId, string jobId)
        {
            SavedJob savedJob = await _context.SavedJobs.FirstOrDefaultAsync(x =>
                x.JobId.ToString() == jobId && x.AspNetUserId == userId && !x.IsDeleted);
            if (savedJob != null)
            {
                savedJob.IsDeleted = true;
                savedJob.DateDeleted = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<SavedJobsModel>> GetSavedJobsAsync(string userId)
        {
            var query = from sj in _context.SavedJobs
                        join j in _context.Jobs on sj.JobId equals j.JobId
                        where sj.AspNetUserId == userId && !sj.IsDeleted
                        join jv in _context.JobViews on j.JobId equals jv.JobId into jobViews
                        from v in jobViews.DefaultIfEmpty()
                        orderby sj.Id descending
                        select new SavedJobsModel
                        {
                            City = j.City,
                            DatePosted = j.DatePosted,
                            ExpireDate = j.ExpireDate,
                            EmployerName = j.EmployerName,
                            JobId = j.JobId,
                            Salary = j.Salary,
                            SalarySummary = j.SalarySummary,
                            Title = j.Title,
                            HoursOfWork = j.HoursOfWork,
                            PeriodOfEmployment = j.PeriodOfEmployment,
                            LastUpdated = j.LastUpdated,
                            PositionsAvailable = j.PositionsAvailable,
                            Source = j.JobSourceId,
                            ExternalSourceName = j.JobSourceId == JobSource.Wanted ? j.OriginalSource : "",
                            ExternalSourceUrl = j.ExternalSourceUrl,
                            ExternalSource = j.ExternalSource,
                            IsActive = j.IsActive,
                            Views = v.Views ?? 0,
                            Id = sj.Id,
                            Note = sj.Note
                        };
            var result = await query?.ToListAsync();
            return result;
        }

        public async Task SaveJobNoteAsync(JobNoteModel jobNoteModel)
        {
            SavedJob savedJob = await _context.SavedJobs.FirstOrDefaultAsync(x =>
                x.JobId.ToString() == jobNoteModel.JobId && x.AspNetUserId == jobNoteModel.UserId && !x.IsDeleted);
            if (savedJob != null && savedJob.Note != jobNoteModel.Note)
            {
                savedJob.Note = jobNoteModel.Note;
                savedJob.NoteUpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}