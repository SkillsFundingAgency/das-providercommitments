using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Exceptions;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService
{
    public class CacheStorageService : ICacheStorageService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheStorageService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SaveToCache<T>(string key, T item, int expirationInHours)
        {
            var json = JsonConvert.SerializeObject(item);

            await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
            });
        }

        public Task SaveToCache<T>(Guid key, T item, int expirationInHours)
        {
            return SaveToCache(key.ToString(), item, expirationInHours);
        }

        public Task SaveToCache<T>(T item, int expirationInHours) where T: ICacheModel
        {
            return SaveToCache(item.CacheKey.ToString(), item, expirationInHours);
        }

        public async Task<T> RetrieveFromCache<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            
            if (json == null)
            {
                throw new CacheItemNotFoundException($"Cache item {key} of type {typeof(T).Name} not found");
            }
            
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Task<T> RetrieveFromCache<T>(Guid key)
        {
            return RetrieveFromCache<T>(key.ToString());
        }

        public async Task DeleteFromCache(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
