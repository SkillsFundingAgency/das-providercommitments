﻿using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]
    class ServiceClaimExtensionsTests
    {
        [TestCase("DAA", true)]
        [TestCase("DAB", true)]
        [TestCase("DAC", true)]
        [TestCase("DAV", true)]
        [TestCase("DA", false)]
        [TestCase("FAKE", false)]
        public void ValidateServiceClaimsTest(string claim, bool expected)
        {
            bool result = claim.IsServiceClaim();
            Assert.AreEqual(expected, result);
        }

        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAA, ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAB, ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAC, ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAC, ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAC, ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAA }, true)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAA }, true)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAA }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAA }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAB }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAB }, true)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAB }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAB }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAC }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAC }, false)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAC }, true)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAC }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { ServiceClaim.DAV }, false)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { ServiceClaim.DAV }, true)]
        [TestCase(ServiceClaim.DAA, new ServiceClaim[] { }, false)]
        [TestCase(ServiceClaim.DAB, new ServiceClaim[] { }, false)]
        [TestCase(ServiceClaim.DAC, new ServiceClaim[] { }, false)]
        [TestCase(ServiceClaim.DAV, new ServiceClaim[] { }, false)]
        public void ShouldReturnHasPermission(ServiceClaim minimumServiceClaim, ServiceClaim[] actualServiceClaims, bool expected)
        {
            var claims = actualServiceClaims.Select(a => new Claim(ProviderClaims.Service, a.ToString()));

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            bool result = claimsPrincipal.HasPermission(minimumServiceClaim);
            Assert.AreEqual(expected, result);
        }
    }
}
