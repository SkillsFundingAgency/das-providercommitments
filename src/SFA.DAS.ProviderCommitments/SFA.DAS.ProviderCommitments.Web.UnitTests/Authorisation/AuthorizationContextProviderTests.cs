using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorisation;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorisation
{
    [TestFixture]
    [Parallelizable]
    public class AuthorizationContextProviderTests
    {
        private AuthorizationContextProviderTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new AuthorizationContextProviderTestsFixture();
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdExistsAndIsValidAndUserIsAuthenticatedAndUkprnIsValid_ThenShouldReturnAuthorizationContextWithAccountLegalEntityIdAndUkprnValues()
        {
            _fixture.SetValidAccountLegalEntityId().SetValidUkprn();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.AccountLegalEntityId, authorizationContext.Get<long?>("AccountLegalEntityId"));
            Assert.AreEqual(_fixture.Ukprn, authorizationContext.Get<long?>("Ukprn"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdDoesNotExistAndUserIsNotAuthenticated_ThenShouldReturnAuthorizationContextWithoutAccountLegalEntityIdAndUkprnValues()
        {
            _fixture.SetUnauthenticatedUser();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsNull(authorizationContext.Get<long?>("AccountLegalEntityId"));
            Assert.IsNull(authorizationContext.Get<long?>("Ukprn"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidAccountLegalEntityId();
            
            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUkprnIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetValidAccountLegalEntityId().SetInvalidUkprn();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
    }

    public class AuthorizationContextProviderTestsFixture
    {
        public IAuthorizationContextProvider AuthorizationContextProvider { get; set; }
        public Mock<IHttpContextAccessor> HttpContextAccessor { get; set; }
        public Mock<IRoutingFeature> RoutingFeature { get; set; }
        public Mock<IPublicAccountLegalEntityIdHashingService> PublicAccountLegalEntityIdHashingService { get; set; }
        public Mock<IAuthenticationService> AuthenticationService { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long Ukprn { get; set; }
        public string UkprnClaimValue { get; set; }

        public AuthorizationContextProviderTestsFixture()
        {
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            RoutingFeature = new Mock<IRoutingFeature>();
            PublicAccountLegalEntityIdHashingService = new Mock<IPublicAccountLegalEntityIdHashingService>();
            AuthenticationService = new Mock<IAuthenticationService>();
            HttpContextAccessor.Setup(c => c.HttpContext.Features[typeof(IRoutingFeature)]).Returns(RoutingFeature.Object);
            RoutingFeature.Setup(f => f.RouteData).Returns(new RouteData());
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Query).Returns(new QueryCollection());
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
            
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContextAccessor.Object, PublicAccountLegalEntityIdHashingService.Object, AuthenticationService.Object);
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            return AuthorizationContextProvider.GetAuthorizationContext();
        }

        public AuthorizationContextProviderTestsFixture SetValidAccountLegalEntityId()
        {
            AccountLegalEntityPublicHashedId = "ABC";
            AccountLegalEntityId = 123;

            var routeData = new RouteData();
            var accountLegalEntityId = AccountLegalEntityId;
            
            routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedId;
            
            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            PublicAccountLegalEntityIdHashingService.Setup(h => h.TryDecodeValue(AccountLegalEntityPublicHashedId, out accountLegalEntityId)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidAccountLegalEntityId()
        {
            AccountLegalEntityPublicHashedId = "AAA";

            var routeData = new RouteData();
            var accountLegalEntityId = 0L;
            
            routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedId;
            
            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            PublicAccountLegalEntityIdHashingService.Setup(h => h.TryDecodeValue(AccountLegalEntityPublicHashedId, out accountLegalEntityId)).Returns(false);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidUkprn()
        {
            Ukprn = 456;
            UkprnClaimValue = Ukprn.ToString();
            
            var ukprnClaimValue = UkprnClaimValue;
            
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidUkprn()
        {
            UkprnClaimValue = "BBB";
            
            var ukprnClaimValue = UkprnClaimValue;
            
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetUnauthenticatedUser()
        {
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(false);
            
            return this;
        }
    }
}