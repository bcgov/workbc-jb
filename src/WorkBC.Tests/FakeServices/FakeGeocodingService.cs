using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Services;

namespace WorkBC.Tests.FakeServices
{
    public class FakeGeocodingService : IGeocodingApiService
    {
        private readonly IConfiguration _configuration;

        public FakeGeocodingService(IConfiguration configuration)
        {
            this._configuration = configuration;

        }

        /// <summary>
        ///     Returns the latitude and longitude for Vancouver
        /// </summary>
        public async Task<GeocodedLocationCache> GetLocation(string location)
        {
            await Task.Delay(0);

            if (location.ToLower() == "surrey")
            {
                return new GeocodedLocationCache
                {
                    Id = int.MaxValue,
                    DateGeocoded = DateTime.Now,
                    Latitude = "49.1913",
                    Longitude = "-122.8490",
                    Name = location
                };
            }

            return new GeocodedLocationCache
            {
                Id = int.MaxValue,
                DateGeocoded = DateTime.Now,
                Latitude = "49.2827",
                Longitude = "-123.1207",
                Name = location
            };
        }
    }
}