using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface ICacheStorageService
    {
        Task<T> RetrieveFromCache<T>(string key);
        Task<T> RetrieveFromCache<T>(Guid key);
        Task SaveToCache<T>(string key, T item, int expirationInHours);
        Task SaveToCache<T>(Guid key, T item, int expirationInHours);
        Task DeleteFromCache(string key);
        
    }
}
