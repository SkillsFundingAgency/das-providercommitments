using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderCommitments.Web.Caching;

public interface IAuthorizationResultCacheConfigurationProvider
{
    Type HandlerType { get; }
    object GetCacheKey(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext);
    void ConfigureCacheEntry(ICacheEntry cacheEntry);
}

public class AuthorizationResultCacheConfigurationProvider : IAuthorizationResultCacheConfigurationProvider
{
    public Type HandlerType { get; } = typeof(AuthorizationHandler);

    public object GetCacheKey(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        return new CacheKey(options, authorizationContext);
    }

    public void ConfigureCacheEntry(ICacheEntry cacheEntry)
    {
        cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(60);
    }
}