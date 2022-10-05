using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace WorkBC.Web.Services
{
    /// <summary>
    ///     Helper methods for accessing the DistributedCache
    /// </summary>
    public class CacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        ///     Checks if a key exists in the cache
        /// </summary>
        public async Task<bool> ExistsAsync(string cacheKey)
        {
            return await _cache.GetAsync(cacheKey) != null;
        }

        /// <summary>
        ///     Gets a string from the cache
        /// </summary>
        public async Task<string> GetString(string cacheKey)
        {
            byte[] b = await _cache.GetAsync(cacheKey);
            return b == null ? null : Encoding.Unicode.GetString(b);
        }

        /// <summary>
        ///     Gets a long from the cache
        /// </summary>
        public async Task<long?> GetLongAsync(string cacheKey)
        {
            byte[] b = await _cache.GetAsync(cacheKey);
            return b == null ? null : (long?) BitConverter.ToInt64(b);
        }

        /// <summary>
        ///     Gets an object from the cache
        /// </summary>
        public async Task<T> GetObjectAsync<T>(string cacheKey)
        {
            byte[] b = await _cache.GetAsync(cacheKey);

            if (b == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.Unicode.GetString(b));
        }

        /// <summary>
        ///     Saves a string to the cache
        /// </summary>
        public async Task SaveStringAsync(string cacheKey, string value, int slidingExpirySeconds = 600)
        {
            await RemoveAsync(cacheKey);

            byte[] b = Encoding.Unicode.GetBytes(value);

            await _cache.SetAsync(cacheKey, b, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(slidingExpirySeconds)
            });
        }

        /// <summary>
        ///     Saves a long to the cache
        /// </summary>
        public async Task SaveLongAsync(string cacheKey, long value, int slidingExpirySeconds = 600)
        {
            await RemoveAsync(cacheKey);

            byte[] b = BitConverter.GetBytes(value);

            await _cache.SetAsync(cacheKey, b, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(slidingExpirySeconds)
            });
        }

        /// <summary>
        ///     Saves an object to the cache
        /// </summary>
        public async Task SaveObjectAsync(string cacheKey, object value, int slidingExpirySeconds = 600)
        {
            await SaveStringAsync(cacheKey, JsonConvert.SerializeObject(value), slidingExpirySeconds);
        }

        /// <summary>
        ///     Removes an item from the cache
        /// </summary>
        public async Task RemoveAsync(string cacheKey)
        {
            if (await _cache.GetAsync(cacheKey) != null)
            {
                await _cache.RemoveAsync(cacheKey);
            }
        }
    }
}