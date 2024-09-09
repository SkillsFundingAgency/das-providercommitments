using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization;

[TestFixture]
[Parallelizable]
public class AuthorizationContextProviderTests
{
    private AuthorizationContextProviderTestsFixture _fixture;

    [SetUp]
    public void SetUp() => _fixture = new AuthorizationContextProviderTestsFixture();

    [Test]
    public void GetAuthorizationContext_WhenValidCohortIdExists_ThenShouldReturnAuthorizationContextWithCohortId()
    {
        var authorizationContext = _fixture.SetValidCohortId().GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet(AuthorizationContextKeys.CohortId, out long cohortId).Should().BeTrue();
        }
    }

    [Test]
    public void GetAuthorizationContext_WhenInvalidCohortIdExists_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetInvalidCohortId();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void
        GetAuthorizationContext_WhenCohortIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutCohortId()
    {
        var authorizationContext = _fixture.GetAuthorizationContext();
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet(AuthorizationContextKeys.CohortId, out long cohortId).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenValidDraftApprenticeshipIdExists_ThenShouldReturnAuthorizationContextWithDraftApprenticeshipId()
    {
        var authorizationContext = _fixture.SetValidDraftApprenticeshipId().GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.Get<long?>(AuthorizationContextKeys.DraftApprenticeshipId).Should().Be(_fixture.DraftApprenticeshipId);
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenInvalidDraftApprenticeshipIdExists_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetInvalidDraftApprenticeship();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void
        GetAuthorizationContext_WhenDraftApprenticeshipIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutDraftApprenticeshipId()
    {
        var authorizationContext = _fixture.GetAuthorizationContext();
            
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet(AuthorizationContextKeys.DraftApprenticeshipId,
                out long draftApprenticeshipId).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndServicesExist_ThenShouldReturnAuthorizationContextWithServices()
    {
        var authorizationContext = _fixture.SetAuthenticatedUser()
            .SetServices()
            .SetUserEmail()
            .SetValidUkprn()
            .GetAuthorizationContext();
            
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.Get<IEnumerable<string>>(AuthorizationContextKeys.Services).Should().BeEquivalentTo(_fixture.Services);
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndServicesDoNotExist_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetAuthenticatedUser();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutServices()
    {
        var authorizationContext = _fixture.GetAuthorizationContext();
            
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet(AuthorizationContextKeys.Services, out long services).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndValidUkprnExistsAndUserEmailExists_ThenShouldReturnAuthorizationContextWithUkprnAndUserEmail()
    {
        var authorizationContext = _fixture.SetAuthenticatedUser()
            .SetServices()
            .SetUserEmail()
            .SetValidUkprn()
            .GetAuthorizationContext();
            
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.Get<long?>("Ukprn").Should().Be(_fixture.Ukprn);
            authorizationContext.Get<string>("UserEmail").Should().Be(_fixture.UserEmail);
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndValidUkprnExistsAndDfEUserEmailExists_ThenShouldReturnAuthorizationContextWithUkprnAndUserEmail()
    {
        var authorizationContext = _fixture.SetAuthenticatedUser()
            .SetServices()
            .SetDfEUserEmail()
            .SetValidUkprn()
            .GetAuthorizationContext();
            
        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.Get<long?>("Ukprn").Should().Be(_fixture.Ukprn);
            authorizationContext.Get<string>("UserEmail").Should().Be(_fixture.DfEUserEmail);
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndUkprnDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetAuthenticatedUser();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndInvalidUkprnExists_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetAuthenticatedUser().SetInvalidUkprn();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutUkprn()
    {
        _fixture.SetUnauthenticatedUser();

        var authorizationContext = _fixture.GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet("Ukprn", out long ukprn).Should().BeFalse();
            authorizationContext.TryGet("UserEmail", out string userEmail).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndUserEmailDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetAuthenticatedUser();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutUserEmail()
    {
        var authorizationContext = _fixture.SetUnauthenticatedUser().GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet("UserEmail", out string userEmail).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenUserIsAuthenticatedAndValidAccountLegalEntityIdExistsAndValidUkprnExists_ThenShouldReturnAuthorizationContextWithAccountLegalEntityIdAndUkprn()
    {
        var authorizationContext = _fixture.SetAuthenticatedUser()
            .SetServices()
            .SetUserEmail()
            .SetValidAccountLegalEntityId()
            .SetValidUkprn()
            .GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.Get<long?>("AccountLegalEntityId").Should().Be(_fixture.AccountLegalEntityId);
            authorizationContext.Get<long?>("Ukprn").Should().Be(_fixture.Ukprn);
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenAccountLegalEntityIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutAccountLegalEntityId()
    {
        var authorizationContext = _fixture.GetAuthorizationContext();

        using (new AssertionScope())
        {
            authorizationContext.Should().NotBeNull();
            authorizationContext.TryGet("AccountLegalEntityId", out long accountLegalEntityId).Should().BeFalse();
        }
    }

    [Test]
    public void
        GetAuthorizationContext_WhenInvalidAccountLegalEntityIdExists_ThenShouldThrowUnauthorizedAccessException()
    {
        _fixture.SetInvalidAccountLegalEntityId();

        var action = () => _fixture.GetAuthorizationContext();
        action.Should().Throw<UnauthorizedAccessException>();
    }
}

public class AuthorizationContextProviderTestsFixture
{
    private readonly RouteData _routeData;
    private readonly Mock<IRoutingFeature> _routingFeature;
    private readonly Mock<IEncodingService> _encodingService;
    private readonly Mock<IAuthenticationService> _authenticationService;
    private readonly AuthorizationContextProvider _authorizationContextProvider;
    private string _accountLegalEntityPublicHashedId;

    private long _cohortId;
    private string _cohortReference;
    private string _ukprnClaimValue;
    private string _draftApprenticeshipHashedId;

    public long DraftApprenticeshipId { get; private set; }
    public List<string> Services { get; private set; }
    public long Ukprn { get; private set; }
    public string UserEmail { get; private set; }
    public string DfEUserEmail { get; private set; }
    public long AccountLegalEntityId { get; private set; }

    public AuthorizationContextProviderTestsFixture()
    {
        _routeData = new RouteData();
        _routingFeature = new Mock<IRoutingFeature>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        _encodingService = new Mock<IEncodingService>();
        _authenticationService = new Mock<IAuthenticationService>();
        _authorizationContextProvider = new AuthorizationContextProvider(httpContextAccessor.Object,
            _encodingService.Object, _authenticationService.Object);

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);

        var featureCollection = new Mock<IFeatureCollection>();
        featureCollection.Setup(f => f.Get<IRoutingFeature>()).Returns(_routingFeature.Object);

        var context = new Mock<HttpContext>();
        context.Setup(c => c.Features).Returns(featureCollection.Object);
        context.Setup(c => c.Request.Query).Returns(new QueryCollection());
        context.Setup(c => c.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));

        httpContextAccessor.Setup(c => c.HttpContext).Returns(context.Object);
    }

    public IAuthorizationContext GetAuthorizationContext()
    {
        return _authorizationContextProvider.GetAuthorizationContext();
    }

    public AuthorizationContextProviderTestsFixture SetAuthenticatedUser()
    {
        _authenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetUnauthenticatedUser()
    {
        _authenticationService.Setup(a => a.IsUserAuthenticated()).Returns(false);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetValidAccountLegalEntityId()
    {
        _accountLegalEntityPublicHashedId = "ABC";
        AccountLegalEntityId = 123;

        var accountLegalEntityId = AccountLegalEntityId;

        _routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = _accountLegalEntityPublicHashedId;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_accountLegalEntityPublicHashedId,
            EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidAccountLegalEntityId()
    {
        _accountLegalEntityPublicHashedId = "AAA";

        var accountLegalEntityId = 0L;

        _routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = _accountLegalEntityPublicHashedId;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_accountLegalEntityPublicHashedId,
            EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(false);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetValidCohortId()
    {
        _cohortReference = "CDE";
        _cohortId = 345;

        var cohortId = _cohortId;

        _routeData.Values[RouteValueKeys.CohortReference] = _cohortReference;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_cohortReference, EncodingType.CohortReference, out cohortId))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidCohortId()
    {
        _cohortReference = "BBB";

        var cohortId = _cohortId;

        _routeData.Values[RouteValueKeys.CohortReference] = _cohortReference;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_cohortReference, EncodingType.CohortReference, out cohortId))
            .Returns(false);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetValidDraftApprenticeshipId()
    {
        _draftApprenticeshipHashedId = "CDE";
        DraftApprenticeshipId = 345;

        var id = DraftApprenticeshipId;

        _routeData.Values[RouteValueKeys.DraftApprenticeshipId] = _draftApprenticeshipHashedId;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_draftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidDraftApprenticeship()
    {
        _draftApprenticeshipHashedId = "BBB";

        var id = DraftApprenticeshipId;

        _routeData.Values[RouteValueKeys.DraftApprenticeshipId] = _draftApprenticeshipHashedId;

        _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
        _encodingService.Setup(h => h.TryDecode(_draftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id))
            .Returns(false);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetValidUkprn()
    {
        Ukprn = 456;
        _ukprnClaimValue = Ukprn.ToString();

        var ukprnClaimValue = _ukprnClaimValue;

        _authenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidUkprn()
    {
        _ukprnClaimValue = "BBB";

        var ukprnClaimValue = _ukprnClaimValue;

        _authenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetServices()
    {
        Services = new List<string>
        {
            "ARA",
            "DAA"
        };

        var services = Services.AsEnumerable();

        _authenticationService.Setup(a => a.TryGetUserClaimValues(ProviderClaims.Service, out services))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetUserEmail()
    {
        UserEmail = "foo@bar.com";

        var userEmailClaimValue = UserEmail;

        _authenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Email, out userEmailClaimValue))
            .Returns(true);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetDfEUserEmail()
    {
        DfEUserEmail = "dfe-foo@bar.com";

        var userEmailClaimValue = DfEUserEmail;

        _authenticationService.Setup(a => a.TryGetUserClaimValue("email", out userEmailClaimValue)).Returns(true);

        return this;
    }
}