using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard.ReportData;

namespace WorkBC.Admin.Areas.Reports.Services
{
    public class JobsByRegionOrSourceReportService
    {
        private readonly DapperContext _dapperContext;
        private readonly JobBoardContext _jobBoardContext;
        private readonly string _externalJobSourceLabel;

        public JobsByRegionOrSourceReportService(JobBoardContext jobBoardContext, DapperContext dapperContext)
        {
            _jobBoardContext = jobBoardContext;
            _dapperContext = dapperContext;
            _externalJobSourceLabel = _jobBoardContext.JobSources.FirstOrDefault(x => x.Id == 2)?.GroupName;
        }

        public async Task GenerateJobsByRegionData(DateTime startDate, DateTime endDate)
        {
            IList<WeeklyPeriod> weeklyPeriods =
                await _jobBoardContext.WeeklyPeriods
                    .Where(p => p.WeekStartDate >= startDate && p.WeekEndDate <= endDate &&
                                p.WeekStartDate < DateTime.Now)
                    .ToListAsync();

            IList<int> weeklyPeriodIds = weeklyPeriods.Select(w => w.Id).ToList();

            IList<ReportPersistenceControl> jobsByRegionPeriods =
                await _jobBoardContext.ReportPersistenceControl
                    .Where(period => 
                        period.TableName == "JobsByRegionOrSource" && 
                        weeklyPeriodIds.Contains(period.WeeklyPeriodId) && !period.IsTotalToDate)
                    .ToListAsync();

            foreach (WeeklyPeriod period in weeklyPeriods)
            {
                if (jobsByRegionPeriods.All(j => j.WeeklyPeriodId != period.Id))
                {
                    // run the stored procedure to populate the missing data
                    await _dapperContext.Persistence.GenerateJobStats(period.WeekEndDate);
                }
            }
        }

        public MatrixReport GroupVacancies(IList<JobStatsResult> results, bool isJobsByRegion = false)
        {
            IEnumerable<IGrouping<string, JobStatsResult>> grouped = results.GroupBy(r => r.Label);

            var vacancies = grouped.Select(grouping => new MatrixRow
                {
                    Label = grouping.Key,
                    Values = grouping.Select(g => g.Vacancies).ToList(),
                    // Jobs By Source is sorted by label, not by vacancies
                    SortOrder = isJobsByRegion
                        ? grouping.First().SortOrder > 1000 ? grouping.First().SortOrder : 0
                        : grouping.Key == _externalJobSourceLabel ? 1 : 0
                }
            ).ToList();

            return new MatrixReport {TableData = vacancies};
        }

        public MatrixReport GroupPostings(IList<JobStatsResult> results, bool isJobsByRegion = false)
        {
            IEnumerable<IGrouping<string, JobStatsResult>> grouped = results.GroupBy(r => r.Label);

            var postings = grouped.Select(grouping => new MatrixRow
                {
                    Label = grouping.Key,
                    Values = grouping.Select(g => g.Postings).ToList(),
                    // Jobs By Source is sorted by label, not by postings
                    SortOrder = isJobsByRegion
                        ? grouping.First().SortOrder > 1000 ? grouping.First().SortOrder : 0
                        : grouping.Key == _externalJobSourceLabel ? 1 : 0
            }
            ).ToList();

            return new MatrixReport {TableData = postings};
        }
    }
}