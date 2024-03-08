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
                .When(BaseUrl + "/career_profile?Noccode=eq.22&limit=1")
                .Respond("application/json",
                    "[{\"CareerProfileId\": 15, \"NameEnglish\": \"some name\", \"Noccode\": \"22\"}]");

            var result = await _service.GetCareerProfileIdByNoc("22");

            Assert.Equal(15, result);
        }
        
        [Fact(DisplayName = "Test the GetCareerProfiles() method returns saved career profiles")]
        public async Task TestGetCareerProfiles()
        {
            const string x = "[{\"CareerProfileId\": 15, \"NameEnglish\": \"some name\", \"Noccode\": \"22\"}," +
                             "{\"CareerProfileId\": 19, \"NameEnglish\": \"other name\", \"Noccode\": \"30\"}]";
            
            _mockHttpMessageHandler
                .When(BaseUrl + $"/career_profile?CareerProfileId=in.(15,19)")
                .Respond("application/json", x);

            var result = await _service.GetCareerProfiles(new List<int>() { 15, 19 });

            Assert.True(result.Count == 2, "GetCareerProfiles did not return 2 records");
            Assert.IsType<CareerProfile>(result[0]);
            Assert.Equivalent(new CareerProfile() {
                CareerProfileId = 15,
                NameEnglish = "some name",
                Noccode = "22"
            }, result[0]);
        }
        
        [Fact(DisplayName = "Test the GetIndustryProfileIdByNaics() method returns the industry profile id")]
        public async Task TestGetIndustryProfileIdByNaics()
        {
            _mockHttpMessageHandler
                .When(BaseUrl + "/industry_profile?naics_id=eq.15&limit=1")
                .Respond("application/json",
                    "[{\"IndustryProfileId\": 15, \"PageTitle\": \"professional services\", \"naics_id\": \"15\"}]");

            var result = await _service.GetIndustryProfileIdByNaics("15");

            Assert.True(result == 15, "GetIndustryProfileIdByNaics() did not return the correct value");
        }
        
        [Fact(DisplayName = "Test the GetSavedIndustryProfiles() method returns saved industry profiles")]
        public async Task TestGetSavedIndustryProfiles()
        {
            const string x = "[{\"IndustryProfileId\": 15, \"PageTitle\": \"professional services\", \"naics_id\": 15}," +
                             "{\"IndustryProfileId\": 16, \"PageTitle\": \"public administration\", \"naics_id\": 16}]";
            
            _mockHttpMessageHandler
                .When(BaseUrl + $"/industry_profile?IndustryProfileId=in.(15,16)")
                .Respond("application/json", x);

            var result = await _service.GetIndustryProfiles(new List<int>() { 15, 16 });

            Assert.True(result.Count == 2, "GetNocsByIndustryProfileIds() did not return 2 records");
            Assert.IsType<IndustryProfile>(result[0]);
            Assert.Equivalent(new IndustryProfile() {
                IndustryProfileId = 15,
                PageTitle = "professional services",
                naics_id = 15
            }, result[0]);
        }

    }
}