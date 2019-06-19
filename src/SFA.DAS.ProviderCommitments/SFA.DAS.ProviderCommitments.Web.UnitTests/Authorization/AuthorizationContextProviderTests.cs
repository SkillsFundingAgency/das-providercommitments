using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization
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
        public void GetAuthorizationContext_WhenAccountLegalEntityIdExistsAndIsValid_ThenShouldReturnAuthorizationContextWithAccountLegalEntityId()
        {
            _fixture.SetValidAccountLegalEntityId();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.AccountLegalEntityId, authorizationContext.Get<long?>("AccountLegalEntityId"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutAccountLegalEntityId()
        {
            _fixture.SetUnauthenticatedUser();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsNull(authorizationContext.Get<long?>("AccountLegalEntityId"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidAccountLegalEntityId();
            
            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenCohortIdExistsAndIsValid_ThenShouldReturnAuthorizationContextWithCohortId()
        {
            _fixture.SetValidCohortId();

            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.CohortId, authorizationContext.Get<long?>("CohortId"));
        }

        [Test]
        public void GetAuthorizationContext_WhenCohortIdExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidCohortId();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenCohortIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutCohortId()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet<long>("CohortId", out var cohortId));
        }

        [Test]
        public void GetAuthorizationContext_WhenDraftApprenticeshipIdExistsAndIsValid_ThenShouldReturnAuthorizationContextWithDraftApprenticeshipId()
        {
            _fixture.SetValidDraftApprenticeshipId();

            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.DraftApprenticeshipId, authorizationContext.Get<long?>("DraftApprenticeshipId"));
        }

        [Test]
        public void GetAuthorizationContext_WhenDraftApprenticeshipIdExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidDraftApprenticeship();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenDraftApprenticeshipIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutDraftApprenticeshipId()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.IsNull(authorizationContext.Get<long?>("DraftApprenticeshipId"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUkprnExistsAndIsValid_ThenShouldReturnAuthorizationContextWithUkprn()
        {
            _fixture.SetValidUkprn().SetValidUserEmail();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.Ukprn, authorizationContext.Get<long?>("Ukprn"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsNotAuthenticated_ThenShouldReturnAuthorizationContextWithoutUkprn()
        {
            _fixture.SetUnauthenticatedUser();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsNull(authorizationContext.Get<long?>("Ukprn"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUkprnDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetValidUserEmail();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUkprnExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidUkprn();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUserEmailExists_ThenShouldReturnAuthorizationContextWithUserEmail()
        {
            _fixture.SetValidUkprn().SetValidUserEmail();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.UserEmail, authorizationContext.Get<string>("UserEmail"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsNotAuthenticated_ThenShouldReturnAuthorizationContextWithoutUserEmail()
        {
            _fixture.SetUnauthenticatedUser();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsNull(authorizationContext.Get<string>("UserEmail"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUserEmailDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetValidUkprn();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
    }

    public class AuthorizationContextProviderTestsFixture
    {
        public IAuthorizationContextProvider AuthorizationContextProvider { get; set; }
        public Mock<IHttpContextAccessor> HttpContextAccessor { get; set; }
        public Mock<IRoutingFeature> RoutingFeature { get; set; }
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<IAuthenticationService> AuthenticationService { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long CohortId { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long Ukprn { get; set; }
        public string UkprnClaimValue { get; set; }
        public string UserEmail { get; set; }


        public AuthorizationContextProviderTestsFixture()
        {
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            RoutingFeature = new Mock<IRoutingFeature>();
            EncodingService = new Mock<IEncodingService>();
            AuthenticationService = new Mock<IAuthenticationService>();
            HttpContextAccessor.Setup(c => c.HttpContext.Features[typeof(IRoutingFeature)]).Returns(RoutingFeature.Object);
            RoutingFeature.Setup(f => f.RouteData).Returns(new RouteData());
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Query).Returns(new QueryCollection());
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
            
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContextAccessor.Object, EncodingService.Object, AuthenticationService.Object);
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
            EncodingService.Setup(h => h.TryDecode(AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidAccountLegalEntityId()
        {
            AccountLegalEntityPublicHashedId = "AAA";

            var routeData = new RouteData();
            var accountLegalEntityId = 0L;
            
            routeData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedId;
            
            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            EncodingService.Setup(h => h.TryDecode(AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(false);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidCohortId()
        {
            CohortReference = "CDE";
            CohortId = 345;

            var routeData = new RouteData();
            var cohortId = CohortId;

            routeData.Values[RouteValueKeys.CohortReference] = CohortReference;

            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            EncodingService.Setup(h => h.TryDecode(CohortReference, EncodingType.CohortReference, out cohortId)).Returns(true);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidDraftApprenticeshipId()
        {
            DraftApprenticeshipHashedId = "CDE";
            DraftApprenticeshipId = 345;

            var routeData = new RouteData();
            var id = DraftApprenticeshipId;

            routeData.Values[RouteValueKeys.DraftApprenticeshipId] = DraftApprenticeshipHashedId;

            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            EncodingService.Setup(h => h.TryDecode(DraftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id)).Returns(true);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidDraftApprenticeship()
        {
            DraftApprenticeshipHashedId = "BBB";

            var routeData = new RouteData();
            var id = DraftApprenticeshipId;

            routeData.Values[RouteValueKeys.DraftApprenticeshipId] = DraftApprenticeshipHashedId;

            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            EncodingService.Setup(h => h.TryDecode(DraftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id)).Returns(false);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidCohortId()
        {
            CohortReference = "BBB";

            var routeData = new RouteData();
            var cohortId = CohortId;

            routeData.Values[RouteValueKeys.CohortReference] = CohortReference;

            RoutingFeature.Setup(f => f.RouteData).Returns(routeData);
            EncodingService.Setup(h => h.TryDecode(CohortReference, EncodingType.CohortReference, out cohortId)).Returns(false);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetUnauthenticatedUser()
        {
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(false);
            
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

        public AuthorizationContextProviderTestsFixture SetValidUserEmail()
        {
            UserEmail = "foo@bar.com";
            
            var userEmailClaimValue = UserEmail;
            
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Email, out userEmailClaimValue)).Returns(true);
            
            return this;
        }
    }
}