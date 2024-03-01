using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.HealthChecks;
using SFA.DAS.Testing;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.HealthChecks
{
    [TestFixture]
    [Parallelizable]
    public class CommitmentsApiHealthCheckTests : FluentTest<CommitmentsApiHealthCheckTestsFixture>
    {
        [TestCase(new[] { Role.Provider }, HealthStatus.Healthy)]
        [TestCase(new[] { Role.Employer }, HealthStatus.Unhealthy)]
        [TestCase(new[] { Role.Provider, Role.Employer }, HealthStatus.Unhealthy)]
        public async Task CheckHealthAsync_WhenWhoAmISucceedsAndUserIsInRoles_ThenShouldReturnHealthStatus(
            string[] roles, HealthStatus healthStatus)
        {
            await TestAsync(
                f => f.SetWhoAmISuccess(roles),
                f => f.CheckHealthAsync(),
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Status.Should().Be(healthStatus);
                    r.Data["Roles"].Should().BeOfType<List<string>>().Which.Should().BeEquivalentTo(roles);
                });
        }

        [Test]
        public async Task CheckHealthAsync_WhenWhoAmIFails_ThenShouldThrowException()
        {
            await TestExceptionAsync(
                f => f.SetWhoAmIFailure(),
                f => f.CheckHealthAsync(),
                (f, r) => r.Should().ThrowAsync<Exception>().Result.Which.Should().Be(f.Exception));
        }
    }

    public class CommitmentsApiHealthCheckTestsFixture
    {
        private readonly HealthCheckContext _healthCheckContext;
        private readonly Mock<ICommitmentsApiClient> _apiClient;
        private readonly CommitmentsApiHealthCheck _healthCheck;
        public RestHttpClientException Exception { get; }

        public CommitmentsApiHealthCheckTestsFixture()
        {
            _healthCheckContext = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("Foo", Mock.Of<IHealthCheck>(), null, null)
            };

            _apiClient = new Mock<ICommitmentsApiClient>();
            _healthCheck = new CommitmentsApiHealthCheck(_apiClient.Object);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                RequestMessage = new HttpRequestMessage(),
                ReasonPhrase = "Url not found"
            };

            Exception = new RestHttpClientException(httpResponseMessage, "Url not found");
        }

        public Task<HealthCheckResult> CheckHealthAsync()
        {
            return _healthCheck.CheckHealthAsync(_healthCheckContext);
        }

        public CommitmentsApiHealthCheckTestsFixture SetWhoAmISuccess(IEnumerable<string> roles)
        {
            _apiClient.Setup(c => c.WhoAmI()).ReturnsAsync(new WhoAmIResponse { Roles = roles.ToList() });
            return this;
        }

        public CommitmentsApiHealthCheckTestsFixture SetWhoAmIFailure()
        {
            _apiClient.Setup(c => c.WhoAmI()).ThrowsAsync(Exception);
            return this;
        }
    }
}