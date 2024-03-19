using Microsoft.Extensions.Caching.Memory;

namespace SFA.DAS.ProviderCommitments.Web.Caching;

public static class MemoryCacheExtensions
{
    public static async Task<T> GetOrCreateAsync<T>(this IMemoryCache memoryCache, object key, Func<ICacheEntry, Task<T>> valueFactory)
    {
        var exists = memoryCache.TryGetValue(key, out T value);

        if (exists)
        {
            return value;
        }

        var cacheEntry = memoryCache.CreateEntry(key);

        value = await valueFactory(cacheEntry).ConfigureAwait(false);

        cacheEntry.SetValue(value);
        cacheEntry.Dispose();

        return value;
    }
}