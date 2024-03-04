using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Caching;
using SFA.DAS.Testing;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Caching;

[TestFixture]
[Parallelizable]
public class AuthorizationContextCacheTests : FluentTest<AuthorizationContextCacheTestsFixture>
{
    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldReturnAuthorizationContext()
    {
        Test(f => f.GetAuthorizationContext(), (f, r) => r.SingleOrDefault().Should().NotBeNull());
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldGetAuthorizationContextFromAuthorizationContextProvider()
    {
        Test(f => f.GetAuthorizationContext(), f => f.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once));
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldGetAuthorizationContextFromAuthorizationContextProviderOnce()
    {
        Test(f => f.GetAuthorizationContext(), f => f.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once));
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldReturnSameAuthorizationContext()
    {
        Test(f => f.GetAuthorizationContext(3), (f, r) => r.ForEach(c => c.Should().Be(r.First())));
    }
}

public class AuthorizationContextCacheTestsFixture
{
    public Mock<IAuthorizationContextProvider> AuthorizationContextProvider { get; set; }
    public IAuthorizationContextProvider AuthorizationContextCache { get; set; }

    public AuthorizationContextCacheTestsFixture()
    {
        AuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
        AuthorizationContextCache = new AuthorizationContextCache(AuthorizationContextProvider.Object);

        AuthorizationContextProvider.Setup(p => p.GetAuthorizationContext()).Returns(() => new AuthorizationContext());
    }

    public List<IAuthorizationContext> GetAuthorizationContext(int count = 1)
    {
        var authorizationContexts = new List<IAuthorizationContext>();
            
        for (var i = 0; i < count; i++)
        {
            authorizationContexts.Add(AuthorizationContextCache.GetAuthorizationContext());
        }

        return authorizationContexts;
    }
}