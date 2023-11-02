﻿using System.Threading.Tasks;
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
        private readonly IGeocodingApiService _geocodingService;

        // constructor
        public GeocodingCachingService(JobBoardContext dbContext, IGeocodingApiService geocodingService,
            ILogger<IGeocodingService> logger = null)
        {
            _dbContext = dbContext;
            _logger = logger;
            _geocodingService = geocodingService;
        }

        public async Task<GeocodedLocationCache> GetLocation(string location)
        {
            GeocodedLocationCache cachedLocation = await GetLocationFromCacheOrNull(_dbContext, location);
                
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
            GeocodedLocationCache cachedLocation = await GetLocationFromCacheOrNull(_dbContext, geo.Name);

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
        
        private static async Task<GeocodedLocationCache> GetLocationFromCacheOrNull(JobBoardContext dbContext, string location)
        {
            return await dbContext.GeocodedLocationCache.FirstOrDefaultAsync(g => g.Name == location);
        }
    }
}