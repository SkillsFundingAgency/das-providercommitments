using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Results;

namespace SFA.DAS.Authorization.Caching
{
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

        private async Task<AuthorizationResult> CreateCacheEntryValue(IAuthorizationHandler authorizationHandler, IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext, IAuthorizationResultCacheConfigurationProvider authorizationResultCacheConfigurationProvider, ICacheEntry cacheEntry)
        {
            var authorizationResult = await authorizationHandler.GetAuthorizationResult(options, authorizationContext).ConfigureAwait(false);

            authorizationResultCacheConfigurationProvider.ConfigureCacheEntry(cacheEntry);

            return authorizationResult;
        }
    }
}