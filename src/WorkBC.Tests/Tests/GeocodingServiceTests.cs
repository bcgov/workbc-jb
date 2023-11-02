using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.Settings;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;
using WorkBC.Tests.FakeServices;
using Xunit;

namespace WorkBC.Tests.Tests
{
    public class GeocodingServiceTests
    {
        //Properties
        private readonly GeocodingCachingService _geocodingCachingService;
        private IGeocodingApiService _geocodingService;
        private JobBoardContext _dbContext;
        private readonly GeocodedLocationCache _deltaLowercase = new GeocodedLocationCache
        {
            DateGeocoded = DateTime.Now,
            Latitude = "49.1913",
            Longitude = "-122.8490",
            Name = "delta, bc"
        };
        private readonly GeocodedLocationCache _deltaUppercase = new GeocodedLocationCache
        {
            DateGeocoded = DateTime.Now,
            Latitude = "49.1913",
            Longitude = "-122.8490",
            Name = "Delta, BC"
        };

        // Constructor
        public GeocodingServiceTests()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<TestsBase>()
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var indexSettings = new IndexSettings();
            var connectionSettings = new ConnectionSettings();

            configuration.GetSection("IndexSettings").Bind(indexSettings);
            configuration.GetSection("ConnectionStrings").Bind(connectionSettings);
            
            // Get database connection string from appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);
            
            //Services
            _geocodingService = new FakeGeocodingService(configuration);
            var logger = new LoggerFactory().CreateLogger<IGeocodingService>();
            _geocodingCachingService = new GeocodingCachingService(_dbContext, _geocodingService, logger);
        }

        [Fact(DisplayName = "Get Geolocation from Cache")]
        public async Task GetGeolocationFromCache()
        {
            await _geocodingCachingService.SetLocation(_deltaLowercase);
            var response = await _geocodingCachingService.GetLocation(_deltaLowercase.Name);
            Assert.Equal(_deltaLowercase, response);
            
            // clean up
            await _geocodingCachingService.DeleteLocation(_deltaLowercase);
            
        }
        
        [Fact(DisplayName = "A Geolocation already in the cache is gracefully handled")]
        public async Task GeolocationInTheCacheAreGracefullyHandled()
        {
            await _geocodingCachingService.SetLocation(_deltaLowercase);
            try
            {
                var response = await _geocodingCachingService.SetLocation(_deltaUppercase);
                Assert.True(true);
            }
            catch (DbUpdateException e)
            {
                // We should NOT get this exception
                const string expected = "Cannot insert duplicate key row in object 'dbo.GeocodedLocationCache' with unique" +
                                    " index 'IX_GeocodedLocationCache_Name'. The duplicate key value is (Delta, BC).";
                Assert.DoesNotContain(expected, e.GetBaseException().Message);
            }
            
            // clean up
            await _geocodingCachingService.DeleteLocation(_deltaLowercase);
            await _geocodingCachingService.DeleteLocation(_deltaUppercase);
        }

        [Fact(DisplayName = "Location names are matched to the cache using using a case-insensitive comparison")]
        public async Task LocationNamesAreMatchedUsingUppercase()
        {
            var mockGeoService = new Mock<IGeocodingApiService>();
            var localCachingService = new GeocodingCachingService(_dbContext, mockGeoService.Object);
            await localCachingService.SetLocation(_deltaLowercase);
            var result = await localCachingService.GetLocation("Delta, BC");
            // We expect GetLocation() to return the result from the cache -- not the result from Moq
            mockGeoService.Verify(service => service.GetLocation(It.IsAny<string>()), Times.Never());
            Assert.Equal(_deltaLowercase.Latitude, result.Latitude);
            
            // clean up
            await _geocodingCachingService.DeleteLocation(_deltaLowercase);
            await _geocodingCachingService.DeleteLocation(_deltaUppercase);
        }
    }
}