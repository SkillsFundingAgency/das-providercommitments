using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderCommitments.Web.Caching;

public class AuthorizationResultCache : IAuthorizationHandler
{
    public string Prefix => _authorizationHandler.Prefix;
        
    private readonly IAuthorizationHandler _authorizationHandler;
    private readonly Dictionary<Type, IAuthorizationResultCacheConfigurationProvider> _authorizationResultCacheConfigurationProviders;
    private readonly IMemoryCache _memoryCache;

    public AuthorizationResultCache(IAuthorizationHandler authorizationHandler, IEnumerable<IAuthorizationResultCacheConfigurationProvider> authorizationResultCacheConfigurationProviders, IMemoryCache memoryCache)
    {
        _authorizationHandler = authorizationHandler;
        _authorizationResultCacheConfigurationProviders = authorizationResultCacheConfigurationProviders.ToDictionary(p => p.HandlerType);
        _memoryCache = memoryCache;
    }

    public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationHandlerType = _authorizationHandler.GetType();
        var authorizationResultCacheConfigurationProvider = _authorizationResultCacheConfigurationProviders[authorizationHandlerType];
        var key = authorizationResultCacheConfigurationProvider.GetCacheKey(options, authorizationContext);
        var authorizationResult = _memoryCache.GetOrCreateAsync(key, e => CreateCacheEntryValue(_authorizationHandler, options, authorizationContext, authorizationResultCacheConfigurationProvider, e));

        return authorizationResult;
    }

    private static async Task<AuthorizationResult> CreateCacheEntryValue(IAuthorizationHandler authorizationHandler, IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext, IAuthorizationResultCacheConfigurationProvider authorizationResultCacheConfigurationProvider, ICacheEntry cacheEntry)
    {
        var authorizationResult = await authorizationHandler.GetAuthorizationResult(options, authorizationContext).ConfigureAwait(false);

        authorizationResultCacheConfigurationProvider.ConfigureCacheEntry(cacheEntry);

        return authorizationResult;
    }
}