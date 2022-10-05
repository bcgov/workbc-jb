using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.ElasticSearch.Indexing.Settings;
using WorkBC.Shared.Settings;
using WorkBC.Shared.Utilities;

namespace WorkBC.Tests.Helpers
{
    public class ElasticSearchSetupHelpers
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly IndexSettings _indexSettings;
        private readonly XmlParsingServiceFederal _xmlServiceFederal;
        private readonly XmlParsingServiceWanted _xmlServiceWanted;
        private readonly IConfiguration _configuration;

        public ElasticSearchSetupHelpers(IConfiguration configuration, ConnectionSettings connectionSettings, 
            IndexSettings indexSettings, XmlParsingServiceFederal xmlServiceFederal, XmlParsingServiceWanted xmlServiceWanted)
        {
            _configuration = configuration;
            _indexSettings = indexSettings;
            _connectionSettings = connectionSettings;
            _xmlServiceFederal = xmlServiceFederal;
            _xmlServiceWanted = xmlServiceWanted;
        }

        /// <summary>
        ///     Setup for tests - create separate elastic search index that we will use for unit tests
        /// </summary>
        public void SetupUnitTestIndex()
        {
            var service = new CreateIndexService(_indexSettings.DefaultIndex, _configuration);
            service.Create(false);
        }

        /// <summary>
        ///     Teardown - Delete elastic search index
        /// </summary>
        public void DestroyUnitTestIndex()
        {
            var service = new DeleteIndexService(_configuration);
            service.Delete(new[] {_indexSettings.DefaultIndex});
        }

        /// <summary>
        ///     Import XML jobs in the fixtures folder
        /// </summary>
        public void ImportFixtures()
        {
            //Read all XML files in the fixtures folder, and index them in the new index.
            var dir = new DirectoryInfo("Fixtures");
            if (dir.Exists)
            {
                //loop through the directories
                foreach (DirectoryInfo folder in dir.GetDirectories())
                {
                    switch (folder.Name.ToLower())
                    {
                        case "federalxmljobs":

                            //get jobs for Federal
                            foreach (FileInfo federalJob in folder.GetFiles())
                            {
                                //job xml as string
                                string jobXml = File.ReadAllText(federalJob.FullName);

                                //index each job
                                ElasticSearchJob esj = _xmlServiceFederal.ConvertToElasticJob(jobXml);

                                //set the expiry date to a future date
                                esj.ExpireDate = DateTime.Now.AddDays(2);

                                //process model to JSON object
                                string jsonFederalJob = JsonConvert.SerializeObject(esj, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });

                                //POST JSON to ElasticSearch
                                string jobId = federalJob.Name.Split('.')[0];
                                PostToElasticSearch(jsonFederalJob, jobId, "PUT",  _indexSettings.DefaultIndex);
                            }

                            break;
                        case "wantedxmljobs":

                            //get jobs for wanted
                            foreach (FileInfo wantedJob in folder.GetFiles())
                            {
                                //job xml as string
                                string jobXml = File.ReadAllText(wantedJob.FullName);

                                //index each job
                                ElasticSearchJob esj = _xmlServiceWanted.ConvertToElasticJob(jobXml);

                                //set the expiry date to a future date
                                esj.ExpireDate = DateTime.Now.AddDays(2);

                                if (esj.JobId == null)
                                {
                                    continue;
                                }

                                //process model to JSON object
                                string jsonWantedJob = JsonConvert.SerializeObject(esj, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });

                                //POST JSON to ElasticSearch
                                string jobId = wantedJob.Name.Split('.')[0];
                                PostToElasticSearch(jsonWantedJob, jobId, "PUT", _indexSettings.DefaultIndex);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     POST data to ElasticSearch
        /// </summary>
        private string PostToElasticSearch(string json, string id, string action, string index)
        {
            string url = $"{_connectionSettings.ElasticSearchServer}/{index}/{_indexSettings.ElasticDocType}/{id}";

            return new ElasticHttpHelper(_indexSettings.ElasticUser, _indexSettings.ElasticPassword)
                .PostToElasticSearch(json, url, action).Result;
        }
    }
}