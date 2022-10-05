using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.Shared.Constants;
using WorkBC.Shared.SystemSettings;

namespace WorkBC.Web.Services
{
    public class SystemSettingsService
    {
        private readonly CacheService _cacheService;
        private readonly IConfiguration _configuration;
        private readonly DateTime _dateInstantiated;

        private Shared.SystemSettings.Settings _settings;


        public SystemSettingsService(CacheService cacheService, IConfiguration configuration)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _dateInstantiated = DateTime.Now;
        }

        /// <summary>
        ///     Gets the "email" settings
        /// </summary>
        public async Task<EmailSettings> EmailSettingsAsync()
        {
            return (await GetAllAsync()).Email;
        }

        /// <summary>
        ///     Gets the "jbAccount" settings
        /// </summary>
        public async Task<JbAccountSettings> JbAccountSettingsAsync()
        {
            return (await GetAllAsync()).JbAccount;
        }

        /// <summary>
        ///     Gets the "jbSearch" settings
        /// </summary>
        public async Task<JbSearchSettings> JbSearchSettingsAsync()
        {
            return (await GetAllAsync()).JbSearch;
        }

        /// <summary>
        ///     Gets all the settings from the distributed cache.  If the settings are not initialized
        ///     in the cache then gets them from the database.
        /// </summary>
        public async Task<Shared.SystemSettings.Settings> GetAllAsync()
        {
            string timestampKey = General.SystemSettingsTimestampCacheKey;
            DateTime? cacheTimestamp = await GetDateCachedAsync();

            if (cacheTimestamp == null)
            {
                cacheTimestamp = DateTime.Now;
                await _cacheService.SaveObjectAsync(timestampKey, cacheTimestamp, General.CacheMinutes * 60);
            }

            if (_settings == null || _settings.CacheTimestamp != cacheTimestamp)
            {
                if (_settings?.CacheTimestamp != cacheTimestamp)
                {
                    cacheTimestamp = DateTime.Now;
                    await _cacheService.SaveObjectAsync(timestampKey, cacheTimestamp, General.CacheMinutes * 60);
                }

                // fix for "cannot consume scoped service ______ from singleton" 
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
                await using var dbContext = new JobBoardContext(connectionString);

                Dictionary<string, string> dictionary = await dbContext
                    .SystemSettings
                    .ToDictionaryAsync(k => k.Name, v => v.Value);

                _settings = new Shared.SystemSettings.Settings(dictionary)
                {
                    CacheTimestamp = cacheTimestamp.Value,
                    SingletonCreated = _dateInstantiated
                };

                // add additional settings (not editable from admin)
                _settings.Shared.Settings.IsProduction = bool.Parse(_configuration["AppSettings:IsProduction"]);
            }

            return _settings;
        }

        /// <summary>
        ///     Gets all the settings from the distributed cache.  If the settings are not initialized
        ///     in the cache then gets them from the database.
        /// </summary>
        private async Task<DateTime?> GetDateCachedAsync()
        {
            return await _cacheService.GetObjectAsync<DateTime?>(General.SystemSettingsTimestampCacheKey);
        }
    }
}