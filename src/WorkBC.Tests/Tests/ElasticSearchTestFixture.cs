using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.ElasticSearch.Indexing.Settings;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;
using WorkBC.Tests.FakeServices;
using WorkBC.Tests.Fixtures;
using WorkBC.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using Location = WorkBC.Data.Model.JobBoard.Location;

namespace WorkBC.Tests.Tests
{
    public class ElasticSearchTestFixture : IDisposable
    {
        protected readonly IConfiguration Configuration;

        //Properties
        protected readonly ElasticSearchSetupHelpers ElasticService;
        protected readonly IGeocodingService GeocodingService;

        // Constructor
        public ElasticSearchTestFixture()
        {
            #region AppSettings

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<TestsHelper>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            var indexSettings = new IndexSettings();
            var connectionSettings = new ConnectionSettings();

            configuration.GetSection("IndexSettings").Bind(indexSettings);
            configuration.GetSection("ConnectionStrings").Bind(connectionSettings);

            Configuration = configuration;

            #endregion

            #region services


            //Services
            GeocodingService = new FakeGeocodingService(Configuration);

            Dictionary<string, string> uniqueCities = FakeCityService.GetUiqueCitiesForIndexing();
            List<Location> duplicateCities = FakeCityService.GetDuplicateCitiesForIndexing();
            List<NocCode> lstNocs = NocFixture.NocCodes;

            var xmlServiceFederal = new XmlParsingServiceFederal(duplicateCities, uniqueCities, lstNocs, GeocodingService);
            var xmlServiceWanted = new XmlParsingServiceWanted(duplicateCities, uniqueCities, lstNocs);

            ElasticService = new ElasticSearchSetupHelpers(
                Configuration,
                connectionSettings, 
                indexSettings, 
                xmlServiceFederal, 
                xmlServiceWanted);

            #endregion

            #region Create TEST index and import test data

            //Elastic Search setup
            ElasticService.SetupUnitTestIndex();

            //Import fixtures
            ElasticService.ImportFixtures();

            // Give ElasticSearch two seconds to process the data before we start searching
            Thread.Sleep(2000);

            #endregion
        }

        public void Dispose()
        {
            //Cleanup
            ElasticService.DestroyUnitTestIndex();
        }


    }
}