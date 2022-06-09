using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CacheService : ICacheService
    {
        IBlobFileTransferClient _blobFile;
        private readonly ILogger<CacheService> _logger;

        public CacheService( IBlobFileTransferClient blobFile, ILogger<CacheService> logger)
        {
            _blobFile = blobFile;
            _logger = logger;
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse =  await _blobFile.DownloadFile(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task<Guid> SetCache<T>(T value, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") where T : class
        {
            var newCacheRequestKey = Guid.NewGuid();
            var response = JsonSerializer.Serialize(value);
            await _blobFile.UploadFile(response, newCacheRequestKey.ToString());
            _logger.LogDebug($"New cahce record created by {memberName} - key : {newCacheRequestKey}");
            return newCacheRequestKey;
        }

        public async Task ClearCache(string key, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            await _blobFile.DeleteFile(key);
            _logger.LogDebug($"Cahced record removed by {memberName} - key : {key}");
        }
    }
}
