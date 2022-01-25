using SFA.DAS.ProviderCommitments.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CacheService : ICacheService
    {
        IBlobFileTransferClient _blobFile;

        public CacheService( IBlobFileTransferClient blobFile)
        {
            _blobFile = blobFile;
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse =  await _blobFile.DownloadFile(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task<Guid> SetCache<T>(T value) where T : class
        {
            var newCacheRequestKey = Guid.NewGuid();
            var response = JsonSerializer.Serialize(value);
            await _blobFile.UploadFile(response, newCacheRequestKey.ToString());
            return newCacheRequestKey;
        }

        public async Task ClearCache(string key)
        {
            await _blobFile.DeleteFile(key);
        }
    }
}
