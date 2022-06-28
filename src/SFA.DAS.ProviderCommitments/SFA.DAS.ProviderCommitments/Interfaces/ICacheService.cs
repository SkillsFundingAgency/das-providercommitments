using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetFromCache<T>(string key) where T : class;
        Task<Guid> SetCache<T>(T value, string memberName = "") where T : class;
        Task ClearCache(string key, string memberName = "");
    }
}