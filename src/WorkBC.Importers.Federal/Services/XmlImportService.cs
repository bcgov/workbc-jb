using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.Importers.Federal.Models;
using WorkBC.Importers.Federal.Settings;
using WorkBC.Shared.Settings;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Importers.Federal.Services
{
    public class XmlImportService
    {
        private const int MaxErrors = 25; // allow up to this many errors per run
        private readonly JobBoardContext _dbContext;
        private readonly FederalSettings _federalSettings;
        private readonly Dictionary<ImportedJobFederal, DateTime> _lstUpdate =
            new Dictionary<ImportedJobFederal, DateTime>();
        private readonly ProxySettings _proxySettings;
        private int _errorCount;
        private Dictionary<long, DateTime> _existingJobsDict = new Dictionary<long, DateTime>();
        private List<JobPosting> _lstInsert = new List<JobPosting>();
        private List<ImportedJobFederal> _lstPurge = new List<ImportedJobFederal>();
        private XmlDocument _xmlDocumentEnglish = new XmlDocument();
        private XmlDocument _xmlDocumentFrench = new XmlDocument();
        private readonly ILogger _logger;
        private readonly CommandLineOptions _options;
        private readonly HttpClient _httpClient;

        public XmlImportService(IConfiguration configuration, CommandLineOptions options, ILogger logger, HttpClient client=null)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);
            _logger = logger;
            _options = options;

            //get API settings
            _federalSettings = new FederalSettings();
            configuration.GetSection("FederalSettings").Bind(_federalSettings);

            //get Proxy settings
            _proxySettings = new ProxySettings();
            configuration.GetSection("ProxySettings").Bind(_proxySettings);

            //create persistent HttpClient if not provided as a dependency
            _httpClient = client ?? CreateHttpClient(_proxySettings, _federalSettings);
        }

        public async Task ProcessJobs(List<JobPosting> apiJobList)
        {
            //get list of jobs in DB
            List<ImportedJobFederal> dbJobs = await _dbContext.ImportedJobsFederal.ToListAsync();
            _existingJobsDict = dbJobs.ToDictionary(j => j.JobId, j => j.ApiDate);

            foreach (var kvpJob in _existingJobsDict)
            {
                var apiJobListEntry = apiJobList.FirstOrDefault(j => j.Id == kvpJob.Key);

                if (apiJobListEntry != null && apiJobListEntry.FileUpdateDate != kvpJob.Value)
                {
                    ImportedJobFederal importedJobFederal = dbJobs.FirstOrDefault(j => j.JobId == apiJobListEntry.Id);
                    if (importedJobFederal != null)
                    {
                        _lstUpdate.Add(importedJobFederal, apiJobListEntry.FileUpdateDate);
                    }
                }
            }

            //Get list of jobs that we need to insert
            _lstInsert = apiJobList.Where(j => dbJobs.All(db => db.JobId != j.Id)).ToList();

            //Insert jobs in list
            _logger.Information($"{_lstInsert.Count} jobs found to insert into the ImportedJobsFederal table");

            if (_options.MaxJobs < _lstInsert.Count)
            {
                _logger.Warning($"MaxJobs is {_options.MaxJobs}.");
            }

            try
            {
                await InsertJobs();
            }
            catch
            {
                _logger.Error("ERROR OCCURRED IMPORTING JOBS... SKIPPING AHEAD TO THE NEXT TASK");
            }

            //Update jobs in list
            _logger.Information($"{_lstUpdate.Count} jobs found to update in the ImportedJobsFederal table");

            if (_options.MaxJobs < _lstUpdate.Count)
            {
                _logger.Warning($"MaxJobs is {_options.MaxJobs}.");
            }

            try
            {
                await UpdateJobs();
            }
            catch
            {
                _logger.Error("ERROR OCCURED UPDATING JOBS... SKIPPING AHEAD TO THE NEXT TASK");
            }

            //get list of jobs in DB
            //Get updated list for delete
            dbJobs = await _dbContext.ImportedJobsFederal.ToListAsync();

            //get a list of all jobs that have expired
            _lstPurge = dbJobs.Where(db => db.DisplayUntil < DateTime.Now).ToList();

            _logger.Information($"{_lstPurge.Count} expired jobs found to purge from the ImportedJobsFederal table");

            //Delete jobs in list
            await PurgeJobs();

            //get a list of all jobs that are in the database but not in the XML
            _lstPurge = dbJobs.Where(db => apiJobList.All(j => j.Id != db.JobId)).ToList();

            _logger.Information($"Purging {_lstPurge.Count} jobs from the ImportedJobsFederal table because they are gone from the Federal JobBank");

            //Delete jobs in list
            await PurgeJobs();
            _logger.Information($"{await _dbContext.ImportedJobsFederal.CountAsync()} jobs in ImportedJobsFederal after import");
        }

        public async Task PurgeJobs()
        {
            try
            {
                var counter = 0;
                foreach (ImportedJobFederal job in _lstPurge.Take(1000))  // never purge more than 1000 jobs at a time (in case the feds are just rebuilding their index and we have bad timing)
                {
                    //check that this job exists in the database
                    ImportedJobFederal jobEntry =
                        await _dbContext.ImportedJobsFederal.FirstOrDefaultAsync(j => j.JobId == job.JobId);

                    if (jobEntry != null)
                    {
                        //ensure there isn't already a job with this ID in the ExpiredJob table
                        ExpiredJob expiredJob = await _dbContext.ExpiredJobs.FirstOrDefaultAsync(j => j.JobId == job.JobId);

                        if (expiredJob == null)
                        {
                            expiredJob = new ExpiredJob
                            {
                                JobId = job.JobId,
                                DateRemoved = DateTime.Now,
                                RemovedFromElasticsearch = false
                            };

                            //Add new row in expired job table
                            await _dbContext.ExpiredJobs.AddAsync(expiredJob);

                            //Remove job from job table
                            _dbContext.ImportedJobsFederal.Remove(job);
                            Console.Write("D");
                            counter++;

                            if (counter >= 500)
                            {
                                //Apply changes
                                await _dbContext.SaveChangesAsync();

                                counter = 0;
                                Console.Write("[500]");
                            }
                        }
                        else
                        {
                            // make sure the expired table has the latest data
                            expiredJob.DateRemoved = DateTime.Now;

                            //update to process this deleted job again
                            expiredJob.RemovedFromElasticsearch = false;
                            _dbContext.ExpiredJobs.Update(expiredJob);

                            //Remove job from ImportedJobsFederal table
                            _dbContext.ImportedJobsFederal.Remove(job);

                            Console.Write("[DUP]");
                        }
                    }
                }

                //update jobs that did not fall in the last 500 jobs
                await _dbContext.SaveChangesAsync();

                // new line at the end of this step
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.Write("Could not delete job. Reason: " + ex.Message + " " + ex.StackTrace);
            }
        }

        public async Task InsertJobs()
        {
            if (_lstInsert.Any())
            {
                Console.WriteLine("I = Inserted  U = Updated  S = Skipped (already imported)  X = Already expired  P = Problem retrieving xml");
            }

            int insertedJobs = 0;
            int expiredJobs = 0;

            foreach (JobPosting job in _lstInsert.Take(_options.MaxJobs))
            {
                if (_errorCount < MaxErrors)
                {
                    try
                    {
                        if (_existingJobsDict.ContainsKey(job.Id) && job.FileUpdateDate == _existingJobsDict[job.Id])
                        {
                            // exit the loop if the job is already inserted
                            Console.Write("S");
                            continue;
                        }

                        //get XML from URL if needed
                        await GetXmlContent(job.Id);

                        if (_xmlDocumentEnglish != null)
                        {
                            //Create DB object
                            DateTime now = DateTime.Now;
                            var existingJobId = _dbContext.JobIds.FirstOrDefault(j => j.Id == job.Id);

                            var federalJob = new ImportedJobFederal
                            {
                                DateFirstImported = existingJobId?.DateFirstImported ?? now,
                                DateLastImported = now,
                                ApiDate = job.FileUpdateDate,
                                JobPostEnglish = _xmlDocumentEnglish.OuterXml,
                                JobPostFrench = _xmlDocumentFrench.OuterXml,
                                JobId = job.Id,
                                ReIndexNeeded = true
                            };

                            // add the job to the JobIds table
                            if (existingJobId == null)
                            {
                                var jobId = new JobId
                                {
                                    Id = job.Id,
                                    DateFirstImported = now,
                                    JobSourceId = JobSource.Federal
                                };
                                await _dbContext.JobIds.AddAsync(jobId);
                                await _dbContext.SaveChangesAsync();
                            }

                            // update display until
                            federalJob.DisplayUntil = GetXmlDisplayUntil(federalJob.JobPostEnglish);

                            //save to DB

                            if (federalJob.DisplayUntil > DateTime.Now)
                            {
                                await _dbContext.ImportedJobsFederal.AddAsync(federalJob);
                                await _dbContext.SaveChangesAsync();
                                Console.Write("I");
                                insertedJobs++;
                            }
                            else
                            {
                                // Jobs that expire today will still appear in the federal feed.  But we don't want to import these
                                Console.Write("X");
                                expiredJobs++;
                            }
                        }
                        else
                        {
                            Console.Write($"P[{job.Id}]");
                        }
                    }
                    catch
                    {
                        _errorCount++;
                        Console.Write("-ERROR_" + _errorCount + "-");
                        if (_errorCount == MaxErrors)
                        {
                            _logger.Error("MAX_ERRORS_" + MaxErrors + "_EXCEEDED");
                        }
                    }
                }
            }

            // new line & summary at the end of this step
            Console.WriteLine();
            _logger.Information($"{insertedJobs} new jobs were inserted into the ImportedJobsFederal table");
            _logger.Information($"{expiredJobs} jobs were already expired");
            _logger.Information($"{_lstInsert.Take(_options.MaxJobs).Count() - insertedJobs - expiredJobs} other jobs were skipped");
            Console.WriteLine();
        }

        public async Task UpdateJobs()
        {
            foreach (KeyValuePair<ImportedJobFederal, DateTime> jobEntry in _lstUpdate.Take(_options.MaxJobs))
            {
                if (_errorCount < MaxErrors)
                {
                    try
                    {
                        ImportedJobFederal federalJob = jobEntry.Key;

                        if (_existingJobsDict.ContainsKey(federalJob.JobId) && federalJob.ApiDate == jobEntry.Value)
                        {
                            // exit the loop if the job is already up to date
                            Console.Write("S");
                            continue;
                        }

                        //get XML from URL if needed
                        await GetXmlContent(federalJob.JobId);

                        if (_xmlDocumentEnglish != null)
                        {
                            //update fields
                            federalJob.ApiDate = jobEntry.Value;
                            federalJob.DateLastImported = DateTime.Now;

                            //update xml content
                            federalJob.JobPostEnglish = _xmlDocumentEnglish.OuterXml;
                            federalJob.JobPostFrench = _xmlDocumentFrench.OuterXml;

                            // update display until
                            federalJob.DisplayUntil = GetXmlDisplayUntil(federalJob.JobPostEnglish);

                            //update record
                            federalJob.ReIndexNeeded = true;
                            _dbContext.ImportedJobsFederal.Update(federalJob);
                            await _dbContext.SaveChangesAsync();

                            Console.Write("U");
                        }
                        else
                        {
                            Console.Write($"P[{jobEntry.Key}]");
                        }
                    }
                    catch
                    {
                        _errorCount++;
                        Console.Write("-ERROR_" + _errorCount + "-");
                        if (_errorCount == MaxErrors)
                        {
                            _logger.Error("MAX_ERRORS_" + MaxErrors + "_EXCEEDED");
                        }
                    }
                }
            }

            // new line at the end of this step
            Console.WriteLine();
        }

        private static HttpClient CreateHttpClient(ProxySettings proxySettings, FederalSettings federalSettings)
        {
            var handler = new HttpClientHandler();

            if (proxySettings.UseProxy)
            {
                handler.Proxy = new WebProxy(proxySettings.ProxyHost, proxySettings.ProxyPort)
                {
                    BypassProxyOnLocal = true
                };
            }

            if (proxySettings.IgnoreSslErrors)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true;
            }

            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var client = new HttpClient(handler);

            // set timeout to 5 seconds
            client.Timeout = new TimeSpan(0,0,5);

            client.DefaultRequestHeaders.Add("Cookie", federalSettings.AuthCookie);
            return client;
        }

        /// <summary>
        ///     Get XML data from URL response
        /// </summary>
        private async Task<XmlDocument> GetWebResponse(string url)
        {
            var xmlData = new XmlDocument();
            var responseFromServer = string.Empty;

            try
            {

                try
                {
                    //Read the web response from URL
                    HttpResponseMessage response = await _httpClient.GetAsync(url);
                    //Save response
                    responseFromServer = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    Console.Write("-WAIT_5_SECONDS-");
                    await Task.Delay(5000); // wait 5 seconds and try again
                    //Read the web response from URL
                    HttpResponseMessage response = await _httpClient.GetAsync(url);
                    //Save response
                    responseFromServer = await response.Content.ReadAsStringAsync();
                }

                //load web request to xml
                xmlData.LoadXml(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                if (responseFromServer.Contains("502 Proxy Error"))
                {
                    // 99% of the time "502 Proxy Error" is the error we get.  It seems to be a problem 
                    // with a proxy server that protects the Federal solr API.
                    _logger.Warning("URL: " + url);
                    _logger.Warning("MESSAGE: 502 Proxy Error");
                }
                else
                {
                    _logger.Error("URL: " + url);
                    _logger.Error("STACKTRACE: " + ex);
                    _logger.Error("RESPONSE: " + responseFromServer);
                }
                xmlData = null;
            }

            //return xml
            return xmlData;
        }

        /// <summary>
        ///     Get a list of all the job Id's available on the federal website.
        /// </summary>
        public async Task<List<JobPosting>> GetAllJobPostingItems()
        {
            var lstJobPostings = new List<JobPosting>();

            try
            {
                //get all job posting ID's from URL
                var province = "/en/bc";
                XmlDocument xmlData = await GetWebResponse(_federalSettings.FederalJobXmlRoot + province + "?includevirtual=true");

                //loop through nodes and create a list of "JobPosting" objects
                if (xmlData != null)
                {
                    //Get the root element
                    XmlElement root = xmlData.DocumentElement;

                    //Number of jobs in this XML
                    var numberOfJobsFound =
                        Convert.ToInt32(root.SelectSingleNode("/SolrResponse/Header/numFound").InnerText);

                    if (numberOfJobsFound > 0)
                    {
                        //Find all documents
                        XmlNodeList nodes = root.SelectNodes("/SolrResponse/Documents/Document");

                        //loop through all documents
                        foreach (XmlNode node in nodes)
                        {
                            //create new JobPosting
                            var jp = new JobPosting
                            {
                                FileUpdateDate = Convert.ToDateTime(node["file_update_date"].InnerText),
                                Id = Convert.ToInt32(node["jobs_id"].InnerText)
                            };

                            //add to list to return
                            lstJobPostings.Add(jp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - GetAllJobPostingItems() : " + ex.Message);
            }

            //return list
            return lstJobPostings;
        }

        /// <summary>
        ///     Get job XML if the job should not be skipped
        /// </summary>
        private async Task GetXmlContent(long jobId)
        {
            _xmlDocumentEnglish = null;
            _xmlDocumentFrench = null;

            var tasks = new Task<XmlDocument>[2];

            tasks[0] = GetWebResponse($"{_federalSettings.FederalJobXmlRoot}/en/{jobId}.xml");
            tasks[1] = GetWebResponse($"{_federalSettings.FederalJobXmlRoot}/fr/{jobId}.xml");

            await Task.WhenAll(tasks);
            _xmlDocumentEnglish = tasks[0].Result;
            _xmlDocumentFrench = tasks[1].Result;
        }

        public DateTime? GetXmlDisplayUntil(string xml)
        {
            //return value - can be null
            DateTime? dt = null;

            //Xml document used to read xml
            var xmlDoc = new XmlDocument();

            //Load XML data in object
            xmlDoc.LoadXml(xml);

            if (xmlDoc.ChildNodes.Count > 0)
            {
                //Get the root element
                XmlElement root = xmlDoc.DocumentElement;

                //Number of jobs in this XML
                //It should be 1
                var numberOfJobsFound =
                    Convert.ToInt32(root.SelectSingleNode("/SolrResponse/Header/numFound").InnerText);

                if (numberOfJobsFound == 1)
                {
                    //Read XML Node
                    XmlNode xmlJobNode = root.SelectSingleNode("/SolrResponse/Documents/Document");

                    //If we have a node
                    if (xmlJobNode != null)
                    {
                        //set date object
                        dt = Convert.ToDateTime(xmlJobNode["display_until"].InnerText);
                    }
                }
            }

            //return object
            return dt;
        }

        public void MigrateDb()
        {
            new MigrationService(_dbContext, _logger).RunDbMigrations();
        }
    }
}