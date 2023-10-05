using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Shared.Services
{
    public class GeocodingCachingService : IGeocodingService
    {
        private readonly JobBoardContext _dbContext;
        private readonly ILogger<IGeocodingService> _logger;
        private readonly IGeocodingService _geocodingService;

        public GeocodingCachingService(JobBoardContext dbContext, IGeocodingService geocodingService,
            ILogger<IGeocodingService> logger = null)
        {
            _dbContext = dbContext;
            _logger = logger;
            _geocodingService = geocodingService;
        }

        public async Task<GeocodedLocationCache> GetLocation(string location)
        {
            GeocodedLocationCache cachedLocation =
                await _dbContext.GeocodedLocationCache.FirstOrDefaultAsync(g => g.Name == location);

            if (cachedLocation != null)
            {
                return cachedLocation;
            }

            // no cache hit; call the geocoding service and save to cache
            var locationObject = await _geocodingService.GetLocation(location);
            return await SetLocation(locationObject);
        }

        public async Task<GeocodedLocationCache> SetLocation(GeocodedLocationCache geo)
        {
            if (geo.Latitude != null && geo.Longitude != null)
            {
                try
                {
                    _dbContext.GeocodedLocationCache.Add(geo);
                    await _dbContext.SaveChangesAsync();
                    return geo;
                }
                catch (DbUpdateException e)
                {
                    // In the event two requests for a uncached location are made near simultaneously
                    // and a duplicate key error is generated, we handle the error gracefully and
                    // return the geoLocation.
                    if (e.GetBaseException().Message.Contains("duplicate key"))
                    {
                        return geo;
                    }
                    
                    _logger?.LogError("DBUpdateException: " + e.Message);
                    throw;
                }
            }
            return null;
        }
        
        public async Task<bool> DeleteLocation(GeocodedLocationCache geo)
        {
            GeocodedLocationCache cachedLocation =
                await _dbContext.GeocodedLocationCache.FirstOrDefaultAsync(g => g.Name == geo.Name);

            if (cachedLocation != null)
            {
                try
                {
                    _dbContext.GeocodedLocationCache.Remove(geo);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException e)
                {
                    _logger?.LogError("DBUpdateException: " + e.Message);
                    throw;
                }
            }
            return false;
        }
    }
}