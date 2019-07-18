using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
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
        public void GetAuthorizationContext_WhenValidCohortIdExists_ThenShouldReturnAuthorizationContextWithCohortId()
        {
            var authorizationContext = _fixture.SetValidCohortId().GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsTrue(authorizationContext.TryGet(AuthorizationContextKeys.CohortId, out long cohortId));
        }

        [Test]
        public void GetAuthorizationContext_WhenInvalidCohortIdExists_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidCohortId();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenCohortIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutCohortId()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet(AuthorizationContextKeys.CohortId, out long cohortId));
        }

        [Test]
        public void GetAuthorizationContext_WhenValidDraftApprenticeshipIdExists_ThenShouldReturnAuthorizationContextWithDraftApprenticeshipId()
        {
            var authorizationContext = _fixture.SetValidDraftApprenticeshipId().GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.DraftApprenticeshipId, authorizationContext.Get<long?>(AuthorizationContextKeys.DraftApprenticeshipId));
        }

        [Test]
        public void GetAuthorizationContext_WhenInvalidDraftApprenticeshipIdExists_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidDraftApprenticeship();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenDraftApprenticeshipIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutDraftApprenticeshipId()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet(AuthorizationContextKeys.DraftApprenticeshipId, out long draftApprenticeshipId));
        }

        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndServicesExist_ThenShouldReturnAuthorizationContextWithServices()
        {
            var authorizationContext = _fixture.SetAuthenticatedUser()
                .SetServices()
                .SetUserEmail()
                .SetValidUkprn()
                .GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.Services, authorizationContext.Get<IEnumerable<string>>(AuthorizationContextKeys.Services));
        }

        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndServicesDoNotExist_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetAuthenticatedUser();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }

        [Test]
        public void GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutServices()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();

            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet(AuthorizationContextKeys.Services, out long services));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndValidUkprnExistsAndUserEmailExists_ThenShouldReturnAuthorizationContextWithUkprnAndUserEmail()
        {
            var authorizationContext = _fixture.SetAuthenticatedUser()
                .SetServices()
                .SetUserEmail()
                .SetValidUkprn()
                .GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.Ukprn, authorizationContext.Get<long?>("Ukprn"));
            Assert.AreEqual(_fixture.UserEmail, authorizationContext.Get<string>("UserEmail"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUkprnDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetAuthenticatedUser();
            
            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndInvalidUkprnExists_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetAuthenticatedUser().SetInvalidUkprn();

            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutUkprn()
        {
            _fixture.SetUnauthenticatedUser();
            
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet("Ukprn", out long ukprn));
            Assert.IsFalse(authorizationContext.TryGet("UserEmail", out string userEmail));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndUserEmailDoesNotExist_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetAuthenticatedUser();
            
            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsUnauthenticated_ThenShouldReturnAuthorizationContextWithoutUserEmail()
        {
            var authorizationContext = _fixture.SetUnauthenticatedUser().GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet("UserEmail", out string userEmail));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenUserIsAuthenticatedAndValidAccountLegalEntityIdExistsAndValidUkprnExists_ThenShouldReturnAuthorizationContextWithAccountLegalEntityIdAndUkprn()
        {
            var authorizationContext = _fixture.SetAuthenticatedUser()
                .SetServices()
                .SetUserEmail()
                .SetValidAccountLegalEntityId()
                .SetValidUkprn()
                .GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.AreEqual(_fixture.AccountLegalEntityId, authorizationContext.Get<long?>("AccountLegalEntityId"));
            Assert.AreEqual(_fixture.Ukprn, authorizationContext.Get<long?>("Ukprn"));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenAccountLegalEntityIdDoesNotExist_ThenShouldReturnAuthorizationContextWithoutAccountLegalEntityId()
        {
            var authorizationContext = _fixture.GetAuthorizationContext();
            
            Assert.IsNotNull(authorizationContext);
            Assert.IsFalse(authorizationContext.TryGet("AccountLegalEntityId", out long accountLegalEntityId));
        }
        
        [Test]
        public void GetAuthorizationContext_WhenInvalidAccountLegalEntityIdExists_ThenShouldThrowUnauthorizedAccessException()
        {
            _fixture.SetInvalidAccountLegalEntityId();
            
            Assert.Throws<UnauthorizedAccessException>(() => _fixture.GetAuthorizationContext());
        }
    }

    public class AuthorizationContextProviderTestsFixture
    {
        public RouteData RouteData { get; set; }
        public Mock<IRoutingFeature> RoutingFeature { get; set; }
        public Mock<IHttpContextAccessor> HttpContextAccessor { get; set; }
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<IAuthenticationService> AuthenticationService { get; set; }
        public AuthorizationContextProvider AuthorizationContextProvider { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long CohortId { get; set; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public List<string> Services { get; set; }
        public long Ukprn { get; set; }
        public string UkprnClaimValue { get; set; }
        public string UserEmail { get; set; }

        public AuthorizationContextProviderTestsFixture()
        {
            RouteData = new RouteData();
            RoutingFeature = new Mock<IRoutingFeature>();
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            EncodingService = new Mock<IEncodingService>();
            AuthenticationService = new Mock<IAuthenticationService>();
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContextAccessor.Object, EncodingService.Object, AuthenticationService.Object);

            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            HttpContextAccessor.Setup(c => c.HttpContext.Features[typeof(IRoutingFeature)]).Returns(RoutingFeature.Object);
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Query).Returns(new QueryCollection());
            HttpContextAccessor.Setup(c => c.HttpContext.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            return AuthorizationContextProvider.GetAuthorizationContext();
        }

        public AuthorizationContextProviderTestsFixture SetAuthenticatedUser()
        {
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetUnauthenticatedUser()
        {
            AuthenticationService.Setup(a => a.IsUserAuthenticated()).Returns(false);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidAccountLegalEntityId()
        {
            AccountLegalEntityPublicHashedId = "ABC";
            AccountLegalEntityId = 123;
            
            var accountLegalEntityId = AccountLegalEntityId;
            
            RouteData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedId;
            
            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidAccountLegalEntityId()
        {
            AccountLegalEntityPublicHashedId = "AAA";
            
            var accountLegalEntityId = 0L;
            
            RouteData.Values[RouteValueKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedId;
            
            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out accountLegalEntityId)).Returns(false);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidCohortId()
        {
            CohortReference = "CDE";
            CohortId = 345;
            
            var cohortId = CohortId;

            RouteData.Values[RouteValueKeys.CohortReference] = CohortReference;

            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(CohortReference, EncodingType.CohortReference, out cohortId)).Returns(true);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidCohortId()
        {
            CohortReference = "BBB";
            
            var cohortId = CohortId;

            RouteData.Values[RouteValueKeys.CohortReference] = CohortReference;

            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(CohortReference, EncodingType.CohortReference, out cohortId)).Returns(false);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidDraftApprenticeshipId()
        {
            DraftApprenticeshipHashedId = "CDE";
            DraftApprenticeshipId = 345;
            
            var id = DraftApprenticeshipId;

            RouteData.Values[RouteValueKeys.DraftApprenticeshipId] = DraftApprenticeshipHashedId;

            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(DraftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id)).Returns(true);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidDraftApprenticeship()
        {
            DraftApprenticeshipHashedId = "BBB";
            
            var id = DraftApprenticeshipId;

            RouteData.Values[RouteValueKeys.DraftApprenticeshipId] = DraftApprenticeshipHashedId;

            RoutingFeature.Setup(f => f.RouteData).Returns(RouteData);
            EncodingService.Setup(h => h.TryDecode(DraftApprenticeshipHashedId, EncodingType.ApprenticeshipId, out id)).Returns(false);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetValidUkprn()
        {
            Ukprn = 456;
            UkprnClaimValue = Ukprn.ToString();
            
            var ukprnClaimValue = UkprnClaimValue;
            
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidUkprn()
        {
            UkprnClaimValue = "BBB";
            
            var ukprnClaimValue = UkprnClaimValue;
            
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Ukprn, out ukprnClaimValue)).Returns(true);
            
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
            
            AuthenticationService.Setup(a => a.TryGetUserClaimValues(ProviderClaims.Service, out services)).Returns(true);
            
            return this;
        }

        public AuthorizationContextProviderTestsFixture SetUserEmail()
        {
            UserEmail = "foo@bar.com";
            
            var userEmailClaimValue = UserEmail;
            
            AuthenticationService.Setup(a => a.TryGetUserClaimValue(ProviderClaims.Email, out userEmailClaimValue)).Returns(true);
            
            return this;
        }
    }
}