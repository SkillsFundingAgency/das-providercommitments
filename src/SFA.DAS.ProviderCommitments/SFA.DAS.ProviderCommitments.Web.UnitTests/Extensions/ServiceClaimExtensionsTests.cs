using System.Linq;
using System.Security.Claims;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

[TestFixture]
internal class ServiceClaimExtensionsTests
{
    [TestCase("DAA", true)]
    [TestCase("DAB", true)]
    [TestCase("DAC", true)]
    [TestCase("DAV", true)]
    [TestCase("DA", false)]
    [TestCase("FAKE", false)]
    public void ValidateServiceClaimsTest(string claim, bool expected)
    {
        var result = claim.IsServiceClaim();
        result.Should().Be(expected);
    }

    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAC, ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAC, ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAC, ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAA }, true)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAA }, true)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAA }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAA }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAB }, false)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAB }, true)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAB }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAB }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAC }, false)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAC }, false)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAC }, true)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAC }, true)]
    [TestCase(ServiceClaim.DAA, new[] { ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAB, new[] { ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAC, new[] { ServiceClaim.DAV }, false)]
    [TestCase(ServiceClaim.DAV, new[] { ServiceClaim.DAV }, true)]
    [TestCase(ServiceClaim.DAA, new ServiceClaim[] { }, false)]
    [TestCase(ServiceClaim.DAB, new ServiceClaim[] { }, false)]
    [TestCase(ServiceClaim.DAC, new ServiceClaim[] { }, false)]
    [TestCase(ServiceClaim.DAV, new ServiceClaim[] { }, false)]
    public void ShouldReturnHasPermission(ServiceClaim minimumServiceClaim, ServiceClaim[] actualServiceClaims, bool expected)
    {
        var claims = actualServiceClaims.Select(a => new Claim(ProviderClaims.Service, a.ToString()));

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var result = claimsPrincipal.HasPermission(minimumServiceClaim);
        result.Should().Be(expected);
    }

    [TestCase(ServiceClaim.DAA, new[] {"DCS", "DCFT"}, false)]
    [TestCase(ServiceClaim.DAB, new[] {"DCS", "DCFT"}, false)]
    [TestCase(ServiceClaim.DAC, new[] {"DCS", "DCFT"}, false)]
    [TestCase(ServiceClaim.DAV, new[] {"DCS", "DCFT"}, false)]
    public void ShouldHandleNonDasClaimPermissions(ServiceClaim minimumServiceClaim, string[] actualServiceClaims, bool expected)
    {
        var claims = actualServiceClaims.Select(a => new Claim(ProviderClaims.Service, a));

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var result = claimsPrincipal.HasPermission(minimumServiceClaim);
        result.Should().Be(expected);
    }
}