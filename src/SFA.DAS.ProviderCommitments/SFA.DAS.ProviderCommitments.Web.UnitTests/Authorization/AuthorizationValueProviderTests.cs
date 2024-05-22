using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization;

public class AuthorizationValueProviderTests
{
    private AuthorizationValueProviderTestFixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new AuthorizationValueProviderTestFixture();
    }

    [Test]
    public void GetCohortId_returns_decoded_CohortId()
    {
        _fixture.SetCohortId(decodingSuccess: true);

        var value = _fixture.Sut.GetCohortId();

        value.Should().NotBe(null);
        value.Should().Be(_fixture.CohortId);
    }

    [Test]
    public void GetCohortId_throws_exception_when_decoding_fails()
    {
        _fixture.SetCohortId(decodingSuccess: false);

        var action = () => _fixture.Sut.GetCohortId();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void GetApprenticeshipId_returns_decoded_ApprenticeshipId()
    {
        _fixture.SetApprenticeshipId(decodingSuccess: true);

        var value = _fixture.Sut.GetApprenticeshipId();

        value.Should().NotBe(null);
        value.Should().Be(_fixture.ApprenticeshipId);
    }

    [Test]
    public void GetApprenticeshipId_throws_exception_when_decoding_fails()
    {
        _fixture.SetApprenticeshipId(decodingSuccess: false);

        var action = () => _fixture.Sut.GetApprenticeshipId();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void GetApprenticeshipId_returns_decoded_AccountLegalEntityId()
    {
        _fixture.SetAccountLegalEntityId(decodingSuccess: true);

        var value = _fixture.Sut.GetAccountLegalEntityId();

        value.Should().NotBe(null);
        value.Should().Be(_fixture.AccountLegalEntityId);
    }

    [Test]
    public void GetAccountLegalEntityId_throws_exception_when_decoding_fails()
    {
        _fixture.SetAccountLegalEntityId(decodingSuccess: false);

        var action = () => _fixture.Sut.GetAccountLegalEntityId();
        action.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void GetProviderId_returns_route_value_when_populated()
    {
        _fixture.SetProviderId();

        var value = _fixture.Sut.GetProviderId();

        value.Should().NotBe(null);
        value.Should().Be(_fixture.ProviderId);
    }
    
    [Test]
    public void GetProviderId_returns_zero_when_routevalue_is_empty()
    {
        _fixture.SetEmptyProviderId();

        var value = _fixture.Sut.GetProviderId();

        value.Should().NotBe(null);
        value.Should().Be(0);
    }

    private class AuthorizationValueProviderTestFixture
    {
        private readonly RouteData _routeData;
        private readonly Mock<IRoutingFeature> _routingFeature;
        private readonly Mock<IEncodingService> _encodingService;

        public long CohortId { get; private set; }
        public long ApprenticeshipId { get; private set; }
        public long AccountLegalEntityId { get; private set; }
        public long ProviderId { get; private set; }

        private string _cohortReference;
        private string _apprenticeshipHashedId;
        private string _accountLegalEntityPublicHashedId;

        public IAuthorizationValueProvider Sut { get; }

        public AuthorizationValueProviderTestFixture()
        {
            _routeData = new RouteData();
            _routingFeature = new Mock<IRoutingFeature>();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            _encodingService = new Mock<IEncodingService>();

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
            var featureCollection = new Mock<IFeatureCollection>();
            featureCollection.Setup(f => f.Get<IRoutingFeature>()).Returns(_routingFeature.Object);

            var context = new Mock<HttpContext>();
            context.Setup(c => c.Features).Returns(featureCollection.Object);
            context.Setup(c => c.Request.Query).Returns(new QueryCollection());
            context.Setup(c => c.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
            httpContextAccessor.Setup(c => c.HttpContext).Returns(context.Object);

            Sut = new AuthorizationValueProvider(httpContextAccessor.Object, _encodingService.Object);
        }

        public AuthorizationValueProviderTestFixture SetCohortId(bool decodingSuccess)
        {
            _cohortReference = "CDE";
            CohortId = 345;

            var cohortId = CohortId;

            _routeData.Values[RouteValueKeys.CohortReference] = _cohortReference;

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
            _encodingService.Setup(h => h.TryDecode(_cohortReference, EncodingType.CohortReference, out cohortId))
                .Returns(decodingSuccess);

            return this;
        }

        public AuthorizationValueProviderTestFixture SetApprenticeshipId(bool decodingSuccess)
        {
            _apprenticeshipHashedId = "HHB";
            ApprenticeshipId = 211;

            var apprenticeshipId = ApprenticeshipId;

            _routeData.Values[RouteValueKeys.ApprenticeshipId] = _apprenticeshipHashedId;

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
            _encodingService.Setup(h => h.TryDecode(_apprenticeshipHashedId, EncodingType.ApprenticeshipId, out apprenticeshipId))
                .Returns(decodingSuccess);

            return this;
        }


        public AuthorizationValueProviderTestFixture SetAccountLegalEntityId(bool decodingSuccess)
        {
            _accountLegalEntityPublicHashedId = "JAK";
            AccountLegalEntityId = 972;

            var accountLegalEntityId = AccountLegalEntityId;

            _routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = _accountLegalEntityPublicHashedId;

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
            _encodingService.Setup(h => h.TryDecode(_accountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId))
                .Returns(decodingSuccess);

            return this;
        }

        public AuthorizationValueProviderTestFixture SetProviderId()
        {
            ProviderId = 433;

            _routeData.Values[RouteValueKeys.ProviderId] = ProviderId.ToString();

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);

            return this;
        }

        public AuthorizationValueProviderTestFixture SetEmptyProviderId()
        {
            _routeData.Values[RouteValueKeys.ProviderId] = string.Empty;

            _routingFeature.Setup(f => f.RouteData).Returns(_routeData);

            return this;
        }
    }
}