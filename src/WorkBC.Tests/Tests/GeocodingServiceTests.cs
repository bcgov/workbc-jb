using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Services;
using WorkBC.Tests.FakeServices;
using Xunit;
using Xunit.Abstractions;

namespace WorkBC.Tests.Tests
{
    public class GeocodingServiceTests : TestsBase
    {
        //Properties
        private readonly GeocodingCachingService _geocodingCachingService;
        private IGeocodingService _geocodingService;
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
        public GeocodingServiceTests(ITestOutputHelper output) : base(output)
        {
            // Get database connection string from appsettings.json
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);
            
            //Services
            _geocodingService = new FakeGeocodingService(Configuration);
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
        
        [Fact(DisplayName = "A Geolocation cannot be saved to the database if an identical lowercase name exists")]
        public async Task LocationNamesAreCaseSensitive()
        {
            await _geocodingCachingService.SetLocation(_deltaLowercase);
            try
            {
                var response = await _geocodingCachingService.SetLocation(_deltaUppercase);
                Assert.True(false);
            }
            catch (Exception e)
            {
                // We should get an exception
                Assert.True(true);
            }
            
            // clean up
            await _geocodingCachingService.DeleteLocation(_deltaLowercase);
            await _geocodingCachingService.DeleteLocation(_deltaUppercase);
        }

        [Fact(DisplayName = "Location names are matched to the cache using using a case-insensitive comparison")]
        public async Task LocationNamesAreMatchedUsingUppercase()
        {
            var notExpectedLocation = new GeocodedLocationCache
            {
                DateGeocoded = DateTime.Now,
                Latitude = "0",
                Longitude = "0",
                Name = "Fictitious, BC"
            };
            
            var mockGeoService = new Mock<IGeocodingService>();
            mockGeoService.Setup(m => m.GetLocation(It.IsAny<string>()))
                .Returns(Task.FromResult(notExpectedLocation));
                
            var localCachingService = new GeocodingCachingService(_dbContext, mockGeoService.Object);
            await localCachingService.SetLocation(_deltaLowercase);
            var result = await localCachingService.GetLocation(_deltaUppercase.Name);
            // We expect GetLocation() to return the result from the cache -- not the result from Moq
            Assert.Equal(_deltaLowercase.Latitude, result.Latitude);
            
            // clean up
            await _geocodingCachingService.DeleteLocation(_deltaLowercase);
            await _geocodingCachingService.DeleteLocation(_deltaUppercase);
        }
        
    }
}