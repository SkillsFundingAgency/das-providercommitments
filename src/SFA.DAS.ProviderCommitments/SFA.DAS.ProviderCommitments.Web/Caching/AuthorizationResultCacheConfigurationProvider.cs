using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Authorization.Caching;
using SFA.DAS.Authorization.CommitmentPermissions.Handlers;
using SFA.DAS.Authorization.Context;

namespace SFA.DAS.Authorization.CommitmentPermissions.Caching
{
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
}