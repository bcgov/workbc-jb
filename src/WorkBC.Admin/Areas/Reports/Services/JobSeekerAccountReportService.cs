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
    public class JobSeekerAccountReportService
    {
        private readonly DapperContext _dapperContext;
        private readonly JobBoardContext _jobBoardContext;

        public JobSeekerAccountReportService(JobBoardContext jobBoardContext, DapperContext dapperContext)
        {
            _jobBoardContext = jobBoardContext;
            _dapperContext = dapperContext;
        }

        public async Task GenerateJobSeekerStats(DateTime startDate, DateTime endDate)
        {
            IList<WeeklyPeriod> weeklyPeriods =
                await _jobBoardContext.WeeklyPeriods
                    .Where(p => p.WeekStartDate >= startDate && p.WeekEndDate <= endDate &&
                                p.WeekStartDate < DateTime.Now)
                    .ToListAsync();

            IList<int> weeklyPeriodIds = weeklyPeriods.Select(w => w.Id).ToList();

            IList<ReportPersistenceControl> jobSeekerStatsPeriods =
                await _jobBoardContext.ReportPersistenceControl
                    .Where(period =>
                        period.TableName == "JobSeekerStats" && 
                        weeklyPeriodIds.Contains(period.WeeklyPeriodId) && !period.IsTotalToDate)
                    .ToListAsync();

            foreach (WeeklyPeriod period in weeklyPeriods)
            {
                if (jobSeekerStatsPeriods.All(j => j.WeeklyPeriodId != period.Id))
                {
                    // run the stored procedure to populate the missing data
                    await _dapperContext.Persistence.GenerateJobSeekerStats(period.WeekEndDate);
                }
            }
        }

        public MatrixReport GroupUsers(IList<JobSeekerAccountResult> results)
        {
            IEnumerable<IGrouping<string, JobSeekerAccountResult>> grouped = results.GroupBy(r => r.Label);

            List<MatrixRow> users = grouped.Select(grouping => new MatrixRow
                {
                    GroupKey = grouping.First().GroupKey,
                    SortOrder = grouping.First().SortOrder,
                    IsTotal = grouping.First().IsTotal,
                    Label = grouping.Key,
                    Values = grouping.Select(g => g.Value).ToList()
                }
            ).ToList();

            return new MatrixReport {TableData = users};
        }

        public async Task<int?> GetMaxPeriod()
        {
            return (await _jobBoardContext.ReportPersistenceControl
                    .FirstOrDefaultAsync(r => r.TableName == "JobSeekerStats" && r.IsTotalToDate))?
                .WeeklyPeriodId;
        }
    }
}