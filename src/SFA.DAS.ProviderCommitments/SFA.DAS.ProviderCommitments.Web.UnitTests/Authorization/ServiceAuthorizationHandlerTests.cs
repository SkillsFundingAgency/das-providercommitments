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
        public async Task GetAuthorizationResult_WhenServiceValueIsValid_ThenShouldReturnAuthorizedAuthorizationResult()
        {
            var authorizationResult = await _fixture.SetValidServiceValue().Handle();
            
            Assert.IsNotNull(authorizationResult);
            Assert.IsTrue(authorizationResult.IsAuthorized);
        }

        [Test]
        public async Task GetAuthorizationResult_WhenServiceValueIsInvalid_ThenShouldReturnUnauthorizedAuthorizationResult()
        {
            var authorizationResult = await _fixture.SetInvalidServiceValue().Handle();
            
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

        public ServiceAuthorizationHandlerTestsFixture SetValidServiceValue()
        {
            AuthorizationContext.Set("Service", new List<string> { "DAA" });
            
            return this;
        }

        public ServiceAuthorizationHandlerTestsFixture SetInvalidServiceValue()
        {
            AuthorizationContext.Set("Service", new List<string> { "Foobar" });

            return this;
        }
    }
}