using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.Handlers;

public class CommitmentsAuthorisationHandlerTests
{
    [Test, MoqAutoData]
    public async Task CanAccessCohortShouldCallOuterApiWhenNoClaimValueAndSaveResultToClaim(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOuterApiService> outerApiService,
        [Frozen] Mock<IAuthorizationValueProvider> authorizationValueProvider,
        CommitmentsAuthorisationHandler sut,
        long providerId,
        long cohortId)
    {
        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST")
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        const bool canAccessCohort = true;

        outerApiService.Setup(x => x.CanAccessCohort(providerId, cohortId)).ReturnsAsync(canAccessCohort);
        authorizationValueProvider.Setup(x => x.GetProviderId()).Returns(providerId);
        authorizationValueProvider.Setup(x => x.GetCohortId()).Returns(cohortId);

        var actual = await sut.CanAccessCohort();

        using (new AssertionScope())
        {
            actual.Should().BeTrue();

            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.AccessibleCohorts)).Should().BeTrue();
            outerApiService.Verify(x => x.CanAccessCohort(providerId, cohortId), Times.Once);

            var claimsValues = JsonConvert.DeserializeObject<Dictionary<long, bool>>(claimsPrinciple.GetClaimValue(ProviderClaims.AccessibleCohorts));
            claimsValues.Single().Key.Should().Be(cohortId);
            claimsValues.Single().Value.Should().Be(canAccessCohort);
        }
    }

    [Test, MoqAutoData]
    public async Task CanAccessCohortShouldNotCallOuterApiWhenAccessIsSavedToClaims(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOuterApiService> outerApiService,
        [Frozen] Mock<IAuthorizationValueProvider> authorizationValueProvider,
        Dictionary<long, bool> existingValues,
        CommitmentsAuthorisationHandler sut,
        long providerId,
        long cohortId)
    {
        const bool canAccessCohort = true;

        existingValues.Add(cohortId, canAccessCohort);

        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST"),
                new Claim(ProviderClaims.AccessibleCohorts, JsonConvert.SerializeObject(existingValues))
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        outerApiService.Setup(x => x.CanAccessCohort(providerId, cohortId)).ReturnsAsync(canAccessCohort);
        authorizationValueProvider.Setup(x => x.GetProviderId()).Returns(providerId);
        authorizationValueProvider.Setup(x => x.GetCohortId()).Returns(cohortId);

        var actual = await sut.CanAccessCohort();

        using (new AssertionScope())
        {
            actual.Should().BeTrue();

            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.AccessibleCohorts)).Should().BeTrue();
            outerApiService.Verify(x => x.CanAccessCohort(providerId, cohortId), Times.Never);

            var claimsValues = JsonConvert.DeserializeObject<Dictionary<long, bool>>(claimsPrinciple.GetClaimValue(ProviderClaims.AccessibleCohorts));
            claimsValues.Count.Should().Be(existingValues.Count);
            claimsValues.Single(x => x.Key == cohortId).Value.Should().Be(canAccessCohort);
        }
    }

    [Test, MoqAutoData]
    public async Task CanAccessApprenticeshipShouldCallOuterApiWhenNoClaimValueAndSaveResultToClaim(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOuterApiService> outerApiService,
        [Frozen] Mock<IAuthorizationValueProvider> authorizationValueProvider,
        CommitmentsAuthorisationHandler sut,
        long providerId,
        long apprenticeshipId)
    {
        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST")
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        const bool canAccessApprenticeship = true;

        outerApiService.Setup(x => x.CanAccessApprenticeship(providerId, apprenticeshipId)).ReturnsAsync(canAccessApprenticeship);
        authorizationValueProvider.Setup(x => x.GetProviderId()).Returns(providerId);
        authorizationValueProvider.Setup(x => x.GetApprenticeshipId()).Returns(apprenticeshipId);

        var actual = await sut.CanAccessApprenticeship();

        using (new AssertionScope())
        {
            actual.Should().BeTrue();

            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.AccessibleApprenticeships)).Should().BeTrue();
            outerApiService.Verify(x => x.CanAccessApprenticeship(providerId, apprenticeshipId), Times.Once);

            var claimsValues = JsonConvert.DeserializeObject<Dictionary<long, bool>>(claimsPrinciple.GetClaimValue(ProviderClaims.AccessibleApprenticeships));
            claimsValues.Single().Key.Should().Be(apprenticeshipId);
            claimsValues.Single().Value.Should().Be(canAccessApprenticeship);
        }
    }
    
    [Test, MoqAutoData]
    public async Task CanAccessApprenticeshipShouldNotCallOuterApiWhenAccessIsSavedToClaims(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOuterApiService> outerApiService,
        [Frozen] Mock<IAuthorizationValueProvider> authorizationValueProvider,
        Dictionary<long, bool> existingValues,
        CommitmentsAuthorisationHandler sut,
        long providerId,
        long apprenticeshipId)
    {
        const bool canAccessApprenticeship = true;

        existingValues.Add(apprenticeshipId, canAccessApprenticeship);

        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST"),
                new Claim(ProviderClaims.AccessibleApprenticeships, JsonConvert.SerializeObject(existingValues))
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        outerApiService.Setup(x => x.CanAccessApprenticeship(providerId, apprenticeshipId)).ReturnsAsync(canAccessApprenticeship);
        authorizationValueProvider.Setup(x => x.GetProviderId()).Returns(providerId);
        authorizationValueProvider.Setup(x => x.GetApprenticeshipId()).Returns(apprenticeshipId);

        var actual = await sut.CanAccessApprenticeship();

        using (new AssertionScope())
        {
            actual.Should().BeTrue();

            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.AccessibleApprenticeships)).Should().BeTrue();
            outerApiService.Verify(x => x.CanAccessCohort(providerId, apprenticeshipId), Times.Never);

            var claimsValues = JsonConvert.DeserializeObject<Dictionary<long, bool>>(claimsPrinciple.GetClaimValue(ProviderClaims.AccessibleApprenticeships));
            claimsValues.Count.Should().Be(existingValues.Count);
            claimsValues.Single(x => x.Key == apprenticeshipId).Value.Should().Be(canAccessApprenticeship);
        }
    }
}