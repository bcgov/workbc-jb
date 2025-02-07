using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.Shared.Extensions;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Importers.Wanted.Services
{
    public class JobsTableSyncService : JobsTableSyncServiceBase
    {
        private readonly XmlParsingServiceWanted _xmlParsingService;
        private readonly int _jobExpiryDays;
        private readonly CommandLineOptions _commandLineOptions;

        public JobsTableSyncService(IConfiguration configuration, ILogger logger, CommandLineOptions commandLineOptions) : base(configuration, logger)
        {
            _jobExpiryDays = int.Parse(configuration["WantedSettings:JobExpiryDays"]);
            _xmlParsingService = new XmlParsingServiceWanted(configuration);
            _commandLineOptions = commandLineOptions;
        }

        public async Task ImportJobs()
        {
            List<long> jobsToImport = DbContext.ImportedJobsWanted
                .Where(ij =>
                    !ij.IsFederalOrWorkBc &&
                    DbContext.Jobs.All(j => j.JobId != ij.JobId) &&
                    DbContext.DeletedJobs.All(dj => dj.JobId != ij.JobId) &&
                    ij.ApiDate.AddDays(_jobExpiryDays) > DateTime.Now)
                .Select(ij => ij.JobId)
                .ToList();

            Logger.Information(jobsToImport.Count() + " jobs found to import");

            foreach (long jobId in jobsToImport)
            {
                ImportedJobWanted importedJob = DbContext.ImportedJobsWanted.First(j => j.JobId == jobId);

                //Load english 
                if (importedJob != null && importedJob.JobPostEnglish.Length > 0)
                {
                    string xmlString = importedJob.JobPostEnglish;

                    ElasticSearchJob elasticJob = _xmlParsingService.ConvertToElasticJob(xmlString);

                    if (elasticJob?.JobId != null)
                    {
                        var job = new Job
                        {
                            JobId = long.Parse(elasticJob.JobId),
                            LastUpdated = DateTime.Now,
                            DateFirstImported = importedJob.DateFirstImported,
                            DateLastImported = importedJob.DateLastImported,
                            JobSourceId = JobSource.Wanted
                        };

                        CopyElasticJob(elasticJob, job);

                        if (elasticJob.ExternalSource.Source.Any())
                        {
                            job.OriginalSource = (elasticJob.ExternalSource.Source[0]?.Source ?? "").Truncate(100);
                            job.ExternalSourceUrl = (elasticJob.ExternalSource.Source[0]?.Url ?? "").Truncate(800);
                        }

                        SetJobTypeFlags(xmlString, job);
                        DateTime version1StartDate = GetVersion1StartDate(elasticJob.DatePosted, importedJob.DateFirstImported);

                        var jobVersion = new JobVersion
                        {
                            JobId = job.JobId,
                            DateVersionStart = version1StartDate,
                            DatePosted = elasticJob.DatePosted,
                            ActualDatePosted = elasticJob.ActualDatePosted,
                            DateFirstImported = version1StartDate,
                            JobSourceId = JobSource.Wanted,
                            IndustryId = job.IndustryId,
                            NocCodeId = job.NocCodeId == 0 ? null : job.NocCodeId,
                            NocCodeId2021 = job.NocCodeId2021 == 0 ? null : job.NocCodeId2021,
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
                    else
                    {
                        importedJob.IsFederalOrWorkBc = true;
                        importedJob.ReIndexNeeded = false;
                        DbContext.ImportedJobsWanted.Update(importedJob);
                        Console.Write("F");
                        await DbContext.SaveChangesAsync();
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
            List<long> jobsToUpdate = (from ij in DbContext.ImportedJobsWanted
                                       join j in DbContext.Jobs on ij.JobId equals j.JobId
                                       where !ij.IsFederalOrWorkBc
                                             && DbContext.DeletedJobs.All(dj => dj.JobId != ij.JobId)
                                             && (j.DateLastImported != ij.DateLastImported || !j.IsActive || _commandLineOptions.ReImport)
                                       select ij.JobId).ToList();

            Logger.Information($"{jobsToUpdate.Count()} jobs found to update");

            foreach (long jobId in jobsToUpdate)
            {
                ImportedJobWanted importedJob = DbContext.ImportedJobsWanted.FirstOrDefault(j => j.JobId == jobId);
                Job job = DbContext.Jobs.FirstOrDefault(j => j.JobId == jobId);

                //Load english 
                if (importedJob != null && importedJob.JobPostEnglish.Length > 0)
                {
                    string xmlString = importedJob.JobPostEnglish;

                    ElasticSearchJob elasticJob = _xmlParsingService.ConvertToElasticJob(xmlString);

                    if (elasticJob?.JobId != null && job != null)
                    {
                        bool needsNewVersion = CopyElasticJob(elasticJob, job);

                        job.LastUpdated = elasticJob.LastUpdated ?? importedJob.DateLastImported;
                        //job.DateFirstImported = importedJob.DateFirstImported;  /* Don't change DateFirstImported or it will mess up reports!! */
                        job.DateLastImported = importedJob.DateLastImported;


                        if (elasticJob.ExternalSource.Source.Any())
                        {
                            job.OriginalSource = (elasticJob.ExternalSource.Source[0]?.Source ?? "").Truncate(100);
                            job.ExternalSourceUrl = (elasticJob.ExternalSource.Source[0]?.Url ?? "").Truncate(800);
                        }

                        SetJobTypeFlags(xmlString, job);

                        if (needsNewVersion)
                        {
                            IncrementJobVersion(job);
                        }

                        DbContext.Jobs.Update(job);

                        Console.Write("U");

                        await DbContext.SaveChangesAsync();

                    }
                    else if (job != null)
                    {
                        job.DateLastImported = importedJob.DateLastImported;
                        job.IsActive = false;
                        IncrementJobVersion(job);
                        DbContext.Jobs.Update(job);
                        importedJob.IsFederalOrWorkBc = true;
                        importedJob.ReIndexNeeded = false;
                        DbContext.ImportedJobsWanted.Update(importedJob);
                        Console.Write("F");
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
            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(xmlString);

            if (xmlDocument.ChildNodes.Count > 0)
            {
                //Read XML Node
                XmlNode xmlJobNode = xmlDocument.SelectSingleNode("job");

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
                    foreach (XmlNode jobType in xmlJobNode.SelectNodes("jobtypes/jobtype"))
                    {
                        string jobTypeStr = jobType.Attributes["label"].InnerText;
                        switch (jobTypeStr.ToLower())
                        {
                            case "full-time":
                                efJob.FullTime = true;
                                break;
                            case "part-time":
                                efJob.PartTime = true;
                                break;
                            case "permanent":
                                efJob.Permanent = true;
                                break;
                            case "contract":
                            case "temporary":
                                efJob.Temporary = true;
                                break;
                        }
                    }
                }
            }
        }
    }
}