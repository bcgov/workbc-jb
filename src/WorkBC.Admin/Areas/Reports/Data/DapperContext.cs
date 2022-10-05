using System;
using Microsoft.EntityFrameworkCore;
using WorkBC.Admin.Areas.Reports.Data.QueryServices;

namespace WorkBC.Admin.Areas.Reports.Data
{
    public class DapperContext : DbContext
    {
        private readonly Lazy<JobsByCityReportQueryService> _jobsByCityReport;
        private readonly Lazy<JobsByIndustryReportQueryService> _jobsByIndustryReport;
        private readonly Lazy<JobsByNocCodeReportQueryService> _jobsByNocCodeReport;
        private readonly Lazy<JobsBySourceReportQueryService> _jobsBySourceReport;
        private readonly Lazy<JobsByRegionReportQueryService> _jobsByRegionReport;
        private readonly Lazy<JobSeekerAccountReportQueryService> _jobSeekerAccountReport;
        private readonly Lazy<JobSeekerDetailReportQueryService> _jobSeekerDetailReport;
        private readonly Lazy<JobSeekersByCityReportQueryService> _jobSeekersByCityReport;
        private readonly Lazy<JobSeekersByLocationReportQueryService> _jobSeekersByLocationReport;
        private readonly Lazy<PersistenceQueryService> _persistence;

        public DapperContext(string connString)
        {
            _jobsByCityReport = new Lazy<JobsByCityReportQueryService>(() => new JobsByCityReportQueryService(connString));
            _jobsByIndustryReport = new Lazy<JobsByIndustryReportQueryService>(() => new JobsByIndustryReportQueryService(connString));
            _jobsByNocCodeReport = new Lazy<JobsByNocCodeReportQueryService>(() => new JobsByNocCodeReportQueryService(connString));
            _jobsBySourceReport = new Lazy<JobsBySourceReportQueryService>(() => new JobsBySourceReportQueryService(connString));
            _jobsByRegionReport = new Lazy<JobsByRegionReportQueryService>(() => new JobsByRegionReportQueryService(connString));
            _jobSeekerAccountReport = new Lazy<JobSeekerAccountReportQueryService>(() => new JobSeekerAccountReportQueryService(connString));
            _jobSeekerDetailReport = new Lazy<JobSeekerDetailReportQueryService>(() => new JobSeekerDetailReportQueryService(connString));
            _jobSeekersByCityReport = new Lazy<JobSeekersByCityReportQueryService>(() => new JobSeekersByCityReportQueryService(connString));
            _jobSeekersByLocationReport = new Lazy<JobSeekersByLocationReportQueryService>(() => new JobSeekersByLocationReportQueryService(connString));
            _persistence = new Lazy<PersistenceQueryService>(() => new PersistenceQueryService(connString));
        }

        public JobsByCityReportQueryService JobsByCity => _jobsByCityReport.Value;
        public JobsByIndustryReportQueryService JobsByIndustry => _jobsByIndustryReport.Value;
        public JobsByNocCodeReportQueryService JobsByNocCode => _jobsByNocCodeReport.Value;
        public JobsBySourceReportQueryService JobsBySource => _jobsBySourceReport.Value;
        public JobsByRegionReportQueryService JobsByRegion => _jobsByRegionReport.Value;
        public JobSeekerAccountReportQueryService JobSeekerAccount => _jobSeekerAccountReport.Value;
        public JobSeekerDetailReportQueryService JobSeekerDetail => _jobSeekerDetailReport.Value;
        public JobSeekersByCityReportQueryService JobSeekersByCity => _jobSeekersByCityReport.Value;
        public JobSeekersByLocationReportQueryService JobSeekersByLocation => _jobSeekersByLocationReport.Value;
        public PersistenceQueryService Persistence => _persistence.Value;
    }
}