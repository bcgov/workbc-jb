using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using MEF = Microsoft.EntityFrameworkCore.EF;

namespace WorkBC.Web.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<IEnumerable<Province>> GetProvincesAsync();
        Task<IEnumerable<dynamic>> GetRegionsAsync(string city);
        Task<IEnumerable<string>> GetCitiesAsync(string city, bool includeRegion);
        Task<IEnumerable<dynamic>> GetAllLocationsAsync();
        Task<IEnumerable<dynamic>> GetAllRegionsAsync();
    }

    public class LocationService : ILocationService
    {
        private const int CacheSeconds = 21600; // 6 hours
        private readonly CacheService _cacheService;
        private readonly JobBoardContext _jobBoardContext;

        public LocationService(JobBoardContext jobBoardContext, CacheService cacheService)
        {
            _jobBoardContext = jobBoardContext;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var cacheKey = "Countries";
            var countries = await _cacheService.GetObjectAsync<IEnumerable<Country>>(cacheKey);

            if (countries != null)
            {
                return countries;
            }

            countries = await _jobBoardContext.Countries.AsNoTracking()
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

            await _cacheService.SaveObjectAsync(cacheKey, countries, CacheSeconds);

            return countries;
        }

        public async Task<IEnumerable<Province>> GetProvincesAsync()
        {
            var cacheKey = "Provinces";
            var provinces = await _cacheService.GetObjectAsync<IEnumerable<Province>>(cacheKey);

            if (provinces != null)
            {
                return provinces;
            }

            provinces = await _jobBoardContext.Provinces.AsNoTracking().ToListAsync();

            await _cacheService.SaveObjectAsync(cacheKey, provinces);

            return provinces;
        }

        public async Task<IEnumerable<dynamic>> GetRegionsAsync(string city)
        {
            string cacheKey = $"City_{city.ToLower()}";

            IEnumerable<dynamic> regions = await _cacheService.GetObjectAsync<IEnumerable<dynamic>>(cacheKey);

            if (regions != null)
            {
                return regions;
            }

            var locations = await _jobBoardContext.Locations.AsNoTracking()
                .Where(x => x.City.ToLower() == city.ToLower() && !x.IsHidden)
                .ToListAsync();

            regions = await GetRegionResult(locations);

            await _cacheService.SaveObjectAsync(cacheKey, regions, CacheSeconds);

            return regions;
        }

        public async Task<IEnumerable<string>> GetCitiesAsync(string city, bool includeRegion)
        {
            string cacheKey = $"Cities_{city.ToLower()}__{(includeRegion ? 1 : 0)}";

            var cities = await _cacheService.GetObjectAsync<IEnumerable<string>>(cacheKey);

            // if (cities != null)
            // {
            //     return cities;
            // }

            cities =  (await _jobBoardContext.Locations.AsNoTracking()
                    .Where(x => !x.IsHidden &&
                                x.LocationId > 0 &&
                                (MEF.Functions.Like(includeRegion ? x.Label : x.City, city + "%") ||
                                 city.Length > 1 &&
                                 MEF.Functions.Like(includeRegion ? x.Label : x.City, "% " + city + "%")))
                    .Select(x => includeRegion ? x.Label : x.City)
                    .Distinct()
                    .ToListAsync())
                .OrderByDescending(x => x.StartsWith(city, StringComparison.CurrentCultureIgnoreCase))
                .ThenBy(x => x)
                .Take(50);

            // cache for 24 hours
            await _cacheService.SaveObjectAsync(cacheKey, cities, CacheSeconds);

            return cities;
        }

        public async Task<IEnumerable<dynamic>> GetAllLocationsAsync()
        {
            var locations = await _jobBoardContext.Locations.Where( l=> !l.IsHidden && l.LocationId > 0)
                            .AsNoTracking()
                            .Distinct()
                            .ToListAsync();

            var result = locations.Select(x => new
            {
                PlaceId = x.LocationId,
                PlaceName = x.Label,
                RegionId = x.RegionId
            }).Distinct().ToList();

            return result;
        }

        public async Task<IEnumerable<dynamic>> GetAllRegionsAsync()
        {
            var regions = await this.GetAllRegions();

            var result = (from region in regions
                          select new { RegionId = region.Key, RegionName = region.Value }).ToList();

            return result;
        }

        private async Task<Dictionary<short, string>> GetAllRegions()
        {
            var cacheKey = "Regions";
            var regions = await _cacheService.GetObjectAsync<Dictionary<short, string>>(cacheKey);

            if (regions != null)
            {
                return regions;
            }

            regions = await _jobBoardContext.Regions.AsNoTracking()
                .Where(r => !r.IsHidden)
                .OrderBy(c => c.ListOrder)
                .ToDictionaryAsync(r => (short) r.Id, r => r.Name);

            await _cacheService.SaveObjectAsync(cacheKey, regions, CacheSeconds);

            return regions;
        }

        private async Task<IEnumerable<dynamic>> GetRegionResult(IList<Location> locations)
        {
            Location[] result = Array.Empty<Location>();

            if (locations == null)
            {
                return result;
            }

            Dictionary<short, string> regions = await GetAllRegions();

            var simpleLocations = locations.Select(x => new
                    {
                        locationId = x.LocationId,
                        locationName = regions.ContainsKey((short) (x.RegionId ?? 0))
                            ? regions[(short) (x.RegionId ?? 0)]
                            : "Unknown Region"
                    }
                ).Distinct()
                .OrderBy(x => x.locationName)
                .ToList();

            return simpleLocations;
        }
    }
}