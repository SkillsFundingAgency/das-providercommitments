using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Caching;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Caching;

[TestFixture]
[Parallelizable]
public class AuthorizationContextCacheTests 
{
    private AuthorizationContextCacheTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new AuthorizationContextCacheTestsFixture();

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldReturnAuthorizationContext()
    {
        var result = _fixture.GetAuthorizationContext();
        result.SingleOrDefault().Should().NotBeNull();
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldGetAuthorizationContextFromAuthorizationContextProvider()
    {
        _fixture.GetAuthorizationContext();
        _fixture.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once);
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldGetAuthorizationContextFromAuthorizationContextProviderOnce()
    {
        _fixture.GetAuthorizationContext();
        _fixture.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once);
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldReturnSameAuthorizationContext()
    {
        var result = _fixture.GetAuthorizationContext(3);
        result.ForEach(c => c.Should().Be(result.First()));
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