using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.ElasticSearch.Indexing.Services;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Importers.Federal.Services
{
    public class JobsTableSyncService : JobsTableSyncServiceBase
    {
        private readonly XmlParsingServiceFederal _xmlParsingService;
        private readonly CommandLineOptions _commandLineOptions;

        public JobsTableSyncService(IConfiguration configuration, ILogger logger, CommandLineOptions commandLineOptions) : base(configuration, logger)
        {
            _xmlParsingService = new XmlParsingServiceFederal(configuration);
            _commandLineOptions = commandLineOptions;
        }

        public async Task ImportJobs()
        {
            List<long> jobsToImport = DbContext.ImportedJobsFederal
                .Where(ij =>
                    DbContext.Jobs.All(j => j.JobId != ij.JobId) && ij.DisplayUntil > DateTime.Now)
                .Select(ij => ij.JobId)
                .ToList();

            Logger.Information(jobsToImport.Count() + " jobs found to import");

            foreach (long jobId in jobsToImport)
            {
                IImportedJob importedJob = DbContext.ImportedJobsFederal.FirstOrDefault(j => j.JobId == jobId);

                //Load english
                if (importedJob != null && importedJob.JobPostEnglish.Length > 0)
                {
                    string xmlString = importedJob.JobPostEnglish;

                    ElasticSearchJob elasticJob = _xmlParsingService.ConvertToElasticJob(xmlString);

                    if (!string.IsNullOrWhiteSpace(elasticJob.JobId))
                    {
                        var job = new Job
                        {
                            JobId = long.Parse(elasticJob.JobId),
                            LastUpdated = DateTime.Now,
                            DateFirstImported = importedJob.DateFirstImported,
                            DateLastImported = importedJob.DateLastImported,
                            OriginalSource = "Federal Job Bank",
                            JobSourceId = JobSource.Federal
                        };

                        CopyElasticJob(elasticJob, job);

                        SetJobTypeFlags(xmlString, job);
                        DateTime version1StartDate = GetVersion1StartDate(elasticJob.DatePosted, importedJob.DateFirstImported);

                        var jobVersion = new JobVersion
                        {
                            JobId = job.JobId,
                            DateVersionStart = version1StartDate,
                            DatePosted = elasticJob.DatePosted,
                            ActualDatePosted = elasticJob.ActualDatePosted,
                            DateFirstImported = version1StartDate,
                            JobSourceId = JobSource.Federal,
                            IndustryId = job.IndustryId,
                            NocCodeId = job.NocCodeId,
                            NocCodeId2021 = job.NocCodeId2021,
                            IsActive = true,
                            PositionsAvailable = job.PositionsAvailable,
                            LocationId = job.LocationId,
                            IsCurrentVersion = true,
                            VersionNumber = 1
                        };

                            DbContext.Jobs.Add(job);
                            DbContext.JobVersions.Add(jobVersion);
                            await DbContext.SaveChangesAsync();


                        Console.Write("I");
                    }
                }
            }

            if (jobsToImport.Any())
            {
                Console.WriteLine();
            }
        }

        public async Task UpdateJobs()
        {
            List<long> jobsToUpdate = (from ij in DbContext.ImportedJobsFederal
                join j in DbContext.Jobs on ij.JobId equals j.JobId
                where j.DateLastImported != ij.DateLastImported || !j.IsActive || _commandLineOptions.ReImport
                select ij.JobId).ToList();

            Logger.Information($"{jobsToUpdate.Count()} jobs found to update");

            foreach (long jobId in jobsToUpdate)
            {
                IImportedJob importedJob = DbContext.ImportedJobsFederal.FirstOrDefault(j => j.JobId == jobId);
                Job job = DbContext.Jobs.FirstOrDefault(j => j.JobId == jobId);

                //Load english
                if (job != null && importedJob != null && importedJob.JobPostEnglish.Length > 0)
                {
                    string xmlString = importedJob.JobPostEnglish;

                    ElasticSearchJob elasticJob = _xmlParsingService.ConvertToElasticJob(xmlString);

                    if (elasticJob != null)
                    {
                        bool needsNewVersion = CopyElasticJob(elasticJob, job);

                        job.LastUpdated = elasticJob.LastUpdated ?? importedJob.DateLastImported;
                        //job.DateFirstImported = importedJob.DateFirstImported;  /* Don't change DateFirstImported or it will mess up reports!! */
                        job.DateLastImported = importedJob.DateLastImported;

                        SetJobTypeFlags(xmlString, job);

                            if (needsNewVersion)
                            {
                                IncrementJobVersion(job);
                            }

                            DbContext.Jobs.Update(job);

                            Console.Write("U");

                            await DbContext.SaveChangesAsync();

                    }
                }
            }

            if (jobsToUpdate.Any())
            {
                Console.WriteLine();
            }
        }

        private static void SetJobTypeFlags(string xmlString, Job efJob)
        {
            //read XML to XmlDocument
            var xmlEnglish = new XmlDocument();

            xmlEnglish.LoadXml(xmlString);

            if (xmlEnglish.ChildNodes.Count > 0)
            {
                //Get the root element
                XmlElement root = xmlEnglish.DocumentElement;

                //Read XML Node
                XmlNode xmlJobNode = root.SelectSingleNode("/SolrResponse/Documents/Document");

                // reset everything to false
                efJob.FullTime = false;
                efJob.PartTime = false;
                efJob.LeadingToFullTime = false;
                efJob.Permanent = false;
                efJob.Temporary = false;
                efJob.Casual = false;
                efJob.Seasonal = false;

                if (xmlJobNode != null)
                {
                    switch (xmlJobNode["work_period_cd"].InnerText)
                    {
                        case "F":
                            efJob.FullTime = true;
                            break;
                        case "P":
                            efJob.PartTime = true;
                            break;
                        case "L":
                            efJob.LeadingToFullTime = true;
                            break;
                    }

                    switch (xmlJobNode["work_term_cd"].InnerText)
                    {
                        case "T":
                            efJob.Temporary = true;
                            break;
                        case "P":
                            efJob.Permanent = true;
                            break;
                        case "C":
                            efJob.Casual = true;
                            break;
                        case "S":
                            efJob.Seasonal = true;
                            break;
                    }
                }
            }
        }
        public async Task<long> GetActiveJobsCount()
        {
            return await DbContext.Jobs.CountAsync(j => j.JobSourceId == JobSource.Federal && j.IsActive);
        }
    }
}