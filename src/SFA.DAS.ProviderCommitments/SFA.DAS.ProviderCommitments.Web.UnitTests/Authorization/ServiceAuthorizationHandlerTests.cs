using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Results;
using SFA.DAS.ProviderCommitments.Web.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable]
    public class ServiceAuthorizationHandlerTests
    {
        private ServiceAuthorizationHandlerTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ServiceAuthorizationHandlerTestsFixture();
        }

        [Test]
        public async Task GetAuthorizationResult_WhenServiceIsValid_ThenShouldReturnAuthorizedAuthorizationResult()
        {
            var authorizationResult = await _fixture.SetValidServices().Handle();
            
            Assert.IsNotNull(authorizationResult);
            Assert.IsTrue(authorizationResult.IsAuthorized);
        }

        [Test]
        public async Task GetAuthorizationResult_WhenServiceIsInvalid_ThenShouldReturnUnauthorizedAuthorizationResult()
        {
            var authorizationResult = await _fixture.SetInvalidServices().Handle();
            
            Assert.IsNotNull(authorizationResult);
            Assert.IsFalse(authorizationResult.IsAuthorized);
            Assert.AreEqual(1, authorizationResult.Errors.Count());
            Assert.IsTrue(authorizationResult.HasError<ServiceNotAuthorized>());
        }
    }

    public class ServiceAuthorizationHandlerTestsFixture
    {
        public List<string> Options { get; set; }
        public AuthorizationContext AuthorizationContext { get; set; }
        public ServiceAuthorizationHandler ServiceAuthorizationHandler { get; set; }

        public ServiceAuthorizationHandlerTestsFixture()
        {
            Options = new List<string>();
            AuthorizationContext = new AuthorizationContext();
            ServiceAuthorizationHandler = new ServiceAuthorizationHandler();
        }

        public Task<AuthorizationResult> Handle()
        {
            return ServiceAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContext);
        }

        public ServiceAuthorizationHandlerTestsFixture SetValidServices()
        {
            AuthorizationContext.Set("Services", new List<string> { "ARA", "DAA" });
            
            return this;
        }

        public ServiceAuthorizationHandlerTestsFixture SetInvalidServices()
        {
            AuthorizationContext.Set("Services", new List<string> { "Foo", "Bar" });
            
            return this;
        }
    }
}