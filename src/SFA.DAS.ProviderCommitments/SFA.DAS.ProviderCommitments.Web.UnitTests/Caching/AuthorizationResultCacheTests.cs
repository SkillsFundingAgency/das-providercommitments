using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Caching;
using SFA.DAS.Testing;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Caching;

[TestFixture]
[Parallelizable]
public class AuthorizationResultCacheTests : FluentTest<AuthorizationHandlerCacheTestsFixture>
{
    [Test]
    public void Prefix_WhenGettingPrefix_ThenShouldReturnAuthorizationHandlerPrefix()
    {
        Test(f => f.AuthorizationResultCache.Prefix, (f, r) => r.Should().Be(f.AuthorizationHandlerPrefix));
    }
        
    [Test]
    public Task GetAuthorizationResult_WhenResultIsNotCached_ThenShouldReturnAuthorizationHandlerResult()
    {
        return TestAsync(f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.AuthorizationResult));
    }

    [Test]
    public Task GetAuthorizationResult_WhenResultIsCached_ThenShouldNotCallHandler()
    {
        return TestAsync(
            f => f.SetCachedAuthorizationResult(),
            f => f.GetAuthorizationResult(),
            f => f.AuthorizationHandler.Verify(h => h.GetAuthorizationResult(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<AuthorizationContext>()), Times.Never));
    }
        
    [Test]
    public Task GetAuthorizationResult_WhenResultIsCached_ThenShouldReturnAuthorizationHandlerResult()
    {
        return TestAsync(
            f => f.SetCachedAuthorizationResult(),
            f => f.GetAuthorizationResult(),
            (f, r) => r.Should().NotBeNull().And.BeSameAs(f.AuthorizationResult));
    }
}

public class AuthorizationHandlerCacheTestsFixture
{
    public IAuthorizationHandler AuthorizationResultCache { get; set; }
    public Mock<IAuthorizationHandler> AuthorizationHandler { get; set; }
    public List<Mock<IAuthorizationResultCacheConfigurationProvider>> AuthorizationResultCachingStrategies { get; set; }
    public Mock<IAuthorizationResultCacheConfigurationProvider> AuthorizationResultCacheConfigurationProvider { get; set; }
    public Mock<IMemoryCache> MemoryCache { get; set; }
    public string AuthorizationHandlerPrefix { get; set; }
    public IReadOnlyCollection<string> Options { get; set; }
    public AuthorizationContext AuthorizationContext { get; set; }
    public object AuthorizationResultCacheKey { get; set; }
    public AuthorizationResult AuthorizationResult { set; get; }

    public AuthorizationHandlerCacheTestsFixture()
    {
        AuthorizationHandler = new Mock<IAuthorizationHandler>();
        AuthorizationResultCacheConfigurationProvider = new Mock<IAuthorizationResultCacheConfigurationProvider>();
        AuthorizationResultCachingStrategies = new List<Mock<IAuthorizationResultCacheConfigurationProvider>> { AuthorizationResultCacheConfigurationProvider };
        MemoryCache = new Mock<IMemoryCache>();
        AuthorizationHandlerPrefix = "Foo.";
        Options = new List<string>();
        AuthorizationContext = new AuthorizationContext();
        AuthorizationResultCacheKey = new object();
        AuthorizationResult = new AuthorizationResult();
            
        AuthorizationHandler.Setup(h => h.Prefix).Returns(AuthorizationHandlerPrefix);
        AuthorizationHandler.Setup(h => h.GetAuthorizationResult(Options, AuthorizationContext)).ReturnsAsync(AuthorizationResult);
        AuthorizationResultCacheConfigurationProvider.Setup(p => p.HandlerType).Returns(AuthorizationHandler.Object.GetType());
        AuthorizationResultCacheConfigurationProvider.Setup(p => p.GetCacheKey(Options, AuthorizationContext)).Returns(AuthorizationResultCacheKey);
        MemoryCache.Setup(mc => mc.CreateEntry(AuthorizationResultCacheKey)).Returns(Mock.Of<ICacheEntry>());
            
        AuthorizationResultCache = new AuthorizationResultCache(AuthorizationHandler.Object, AuthorizationResultCachingStrategies.Select(m => m.Object), MemoryCache.Object);
    }

    public AuthorizationHandlerCacheTestsFixture SetCachedAuthorizationResult()
    {
        object value = AuthorizationResult;
            
        MemoryCache.Setup(c => c.TryGetValue(AuthorizationResultCacheKey, out value)).Returns(true);
            
        return this;
    }

    public Task<AuthorizationResult> GetAuthorizationResult()
    {
        return AuthorizationResultCache.GetAuthorizationResult(Options, AuthorizationContext);
    }
}