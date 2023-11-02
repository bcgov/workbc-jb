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
using Xunit.Abstractions;
using Location = WorkBC.Data.Model.JobBoard.Location;

namespace WorkBC.Tests.Tests
{
    public abstract class TestsHelper
    {

        public static async Task<List<Source>> QueryElasticSearch(JobSearchQuery esq)
        {
            //Get search results from Elastic search
            ElasticSearchResponse results = await esq.GetSearchResults();

            //Read results from Elastic
            if (results != null)
            {
                if (results.Hits != null && results.Hits.HitsHits != null)
                {
                    //Return results to test
                    return results.Hits.HitsHits.Select(hit => hit.Source).ToArray().ToList();
                }
            }

            //If no results, return empty result set
            return new List<Source>();
        }

        public static IConfiguration GetConfiguration()
        {
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

            return configuration;
        }
        
    }
}