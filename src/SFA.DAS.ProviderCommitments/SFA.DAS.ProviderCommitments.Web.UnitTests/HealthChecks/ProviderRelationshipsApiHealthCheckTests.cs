using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.HealthChecks;
using SFA.DAS.ProviderRelationships.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.HealthChecks
{
    [TestFixture]
    [Parallelizable]
    public class ProviderRelationshipsApiHealthCheckTests
    {
        private ProviderRelationshipsApiHealthCheckTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ProviderRelationshipsApiHealthCheckTestsFixture();
        }
        
        [Test]
        public async Task CheckHealthAsync_WhenPingSucceeds_ThenShouldReturnHealthyStatus()
        {
            var healthCheckResult = await _fixture.SetPingSuccess().CheckHealthAsync();
            
            Assert.AreEqual(HealthStatus.Healthy, healthCheckResult.Status);
        }
        
        [Test]
        public async Task CheckHealthAsync_WhenPingFails_ThenShouldReturnDegradedStatus()
        {
            var healthCheckResult = await _fixture.SetPingFailure().CheckHealthAsync();
            
            Assert.AreEqual(HealthStatus.Degraded, healthCheckResult.Status);
            Assert.AreEqual(_fixture.Exception.Message, healthCheckResult.Description);
        }

        private class ProviderRelationshipsApiHealthCheckTestsFixture
        {
            public HealthCheckContext HealthCheckContext { get; set; }
            public CancellationToken CancellationToken { get; set; }
            public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; set; }
            public ProviderRelationshipsApiHealthCheck HealthCheck { get; set; }
            public Exception Exception { get; set; }

            public ProviderRelationshipsApiHealthCheckTestsFixture()
            {
                HealthCheckContext = new HealthCheckContext
                {
                    Registration = new HealthCheckRegistration("Foo", Mock.Of<IHealthCheck>(), null, null)
                };
                
                ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();
                HealthCheck = new ProviderRelationshipsApiHealthCheck(ProviderRelationshipsApiClient.Object);
                Exception = new Exception("Foobar");
            }
            
            public Task<HealthCheckResult> CheckHealthAsync()
            {
                return HealthCheck.CheckHealthAsync(HealthCheckContext, CancellationToken);
            }

            public ProviderRelationshipsApiHealthCheckTestsFixture SetPingSuccess()
            {
                ProviderRelationshipsApiClient.Setup(c => c.Ping(CancellationToken)).Returns(Task.CompletedTask);
                
                return this;
            }

            public ProviderRelationshipsApiHealthCheckTestsFixture SetPingFailure()
            {
                ProviderRelationshipsApiClient.Setup(c => c.Ping(CancellationToken)).ThrowsAsync(Exception);
                
                return this;
            }
        }
    }
}