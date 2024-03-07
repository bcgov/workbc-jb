using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using WorkBC.Data;
using Xunit;
using RichardSzalay.MockHttp;
using WorkBC.Data.Model.Ssot;

namespace WorkBC.Tests.Tests
{
    public class SsotApiTests
    {
        //Properties
        private readonly SsotApi _service;
        private readonly HttpClient _httpClient;
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private const string BaseUrl = "http://localhost:3000";

        public SsotApiTests()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttpMessageHandler);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            
            _service = new SsotApi(configuration, _httpClient);
        }

        [Fact(DisplayName = "Test the GetCareerProfileIdByNoc() method returns the career profile id")]
        public async Task TestGetCareerProfileIdByNoc()
        {
            _mockHttpMessageHandler
                .When(BaseUrl + "/career_profile?noccode=eq.22&limit=1")
                .Respond("application/json",
                    "[{\"careerprofileid\": 15, \"nameenglish\": \"some name\", \"noccode\": \"22\"}]");

            var result = await _service.GetCareerProfileIdByNoc("22");

            Assert.True(result == 15, "GetCareerProfileIdByNoc did not return the correct value");
        }
        
        [Fact(DisplayName = "Test the GetNocsByCareerProfileIds() method returns the nocs")]
        public async Task TestGetNocsByCareerProfileIds()
        {
            const string x = "[{\"careerprofileid\": 15, \"nameenglish\": \"some name\", \"noccode\": \"22\"}," +
                             "{\"careerprofileid\": 19, \"nameenglish\": \"other name\", \"noccode\": \"30\"}]";
            
            _mockHttpMessageHandler
                .When(BaseUrl + $"/career_profile?careerprofileid=in.(15,19)")
                .Respond("application/json", x);

            var result = await _service.GetNocsByCareerProfileIds(new List<int>() { 15, 19 });

            Assert.True(result.Count == 2, "GetNocsByCareerProfileIds did not return 2 records");
            Assert.IsType<CareerProfile>(result[0]);
        }
        
        [Fact(DisplayName = "Test the GetIndustryProfileIdByNaics() method returns the industry profile id")]
        public async Task TestGetIndustryProfileIdByNaics()
        {
            _mockHttpMessageHandler
                .When(BaseUrl + "/industry_profile?naics_id=eq.15&limit=1")
                .Respond("application/json",
                    "[{\"industryprofileid\": 15, \"pagetitle\": \"professional services\", \"naics_id\": \"15\"}]");

            var result = await _service.GetIndustryProfileIdByNaics("15");

            Assert.True(result == 15, "GetIndustryProfileIdByNaics() did not return the correct value");
        }
        
        [Fact(DisplayName = "Test the GetNocsByIndustryProfileIds() method returns the nocs")]
        public async Task TestGetNocsByIndustryProfileIds()
        {
            const string x = "[{\"industryprofileid\": 15, \"pagetitle\": \"professional services\", \"naics_id\": \"15\"}," +
                             "{\"industryprofileid\": 16, \"pagetitle\": \"public administration\", \"naics_id\": \"16\"}]";
            
            _mockHttpMessageHandler
                .When(BaseUrl + $"/industry_profile?industryprofileid=in.(15,16)")
                .Respond("application/json", x);

            var result = await _service.GetNocsByIndustryProfileIds(new List<int>() { 15, 16 });

            Assert.True(result.Count == 2, "GetNocsByIndustryProfileIds() did not return 2 records");
            Assert.IsType<IndustryProfile>(result[0]);
        }

    }
}