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

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.HealthChecks;

[TestFixture]
[Parallelizable]
public class CommitmentsApiHealthCheckTests
{
    private CommitmentsApiHealthCheckTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new CommitmentsApiHealthCheckTestsFixture();

    [TestCase(new[] { Role.Provider }, HealthStatus.Healthy)]
    [TestCase(new[] { Role.Employer }, HealthStatus.Unhealthy)]
    [TestCase(new[] { Role.Provider, Role.Employer }, HealthStatus.Unhealthy)]
    public async Task CheckHealthAsync_WhenWhoAmISucceedsAndUserIsInRoles_ThenShouldReturnHealthStatus(string[] roles, HealthStatus healthStatus)
    {
        _fixture.SetWhoAmISuccess(roles);

        var result = await _fixture.CheckHealthAsync();

        result.Should().NotBeNull();
        result.Status.Should().Be(healthStatus);
        result.Data["Roles"].Should().BeOfType<List<string>>().Which.Should().BeEquivalentTo(roles);
    }

    [Test]
    public void CheckHealthAsync_WhenWhoAmIFails_ThenShouldThrowException()
    {
        _fixture.SetWhoAmIFailure();
        
        var act = async () => await _fixture.CheckHealthAsync();
        
        act.Should().ThrowAsync<Exception>().Result.Which.Should().Be(_fixture.Exception);
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