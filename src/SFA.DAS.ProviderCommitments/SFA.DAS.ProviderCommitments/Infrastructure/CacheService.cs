using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.ProviderCommitments.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        DistributedCacheEntryOptions _options;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
            _options = new DistributedCacheEntryOptions();
            _options.SlidingExpiration = new TimeSpan(TimeSpan.TicksPerHour * 3);
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task<Guid> SetCache<T>(T value) where T : class
        {
            var newCacheRequestKey = Guid.NewGuid();
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(newCacheRequestKey.ToString(), response, _options);
            return newCacheRequestKey;
        }

        public async Task ClearCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
