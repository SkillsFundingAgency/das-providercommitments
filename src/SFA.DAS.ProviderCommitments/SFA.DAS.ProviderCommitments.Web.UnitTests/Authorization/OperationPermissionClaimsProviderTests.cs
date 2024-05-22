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
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization;

public class OperationPermissionClaimClaimsProviderTests
{
    [Test, MoqAutoData]
    public void ShouldSaveToNewClaim(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        OperationPermissionClaimClaimsProvider sut,
        OperationPermission permission)
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

        sut.Save(permission);

        using (new AssertionScope())
        {
            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.OperationPermissions)).Should().BeTrue();

            var claimsValues = JsonConvert.DeserializeObject<List<OperationPermission>>(claimsPrinciple.GetClaimValue(ProviderClaims.OperationPermissions));
            claimsValues.First().Should().Be(permission);
        }
    }

    [Test, MoqAutoData]
    public void ShouldUpdateExistingClaim(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        OperationPermissionClaimClaimsProvider sut,
        List<OperationPermission> existingPermissions,
        OperationPermission permission)
    {
        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST"),
                new Claim(ProviderClaims.OperationPermissions, JsonConvert.SerializeObject(existingPermissions))
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        sut.Save(permission);

        using (new AssertionScope())
        {
            claimsPrinciple.HasClaim(x => x.Type.Equals(ProviderClaims.OperationPermissions)).Should().BeTrue();

            var claimsValues = JsonConvert.DeserializeObject<List<OperationPermission>>(claimsPrinciple.GetClaimValue(ProviderClaims.OperationPermissions));
            claimsValues.Should().Contain(permission);
            claimsValues.Count.Should().Be(existingPermissions.Count + 1);
        }
    }

    [Test, MoqAutoData]
    public void ShouldReturnFalseFromTryGetWhenThereIsNoPermissionsClaims(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        OperationPermissionClaimClaimsProvider sut,
        long? accountLegalEntityId,
        Operation operation)
    {
        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST"),
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var actual = sut.TryGetPermission(accountLegalEntityId, operation, out var result);

        using (new AssertionScope())
        {
            actual.Should().BeFalse();
            result.Should().BeFalse();
        }
    }
    
    [Test, MoqAutoData]
    public void ShouldReturnTrueAndReturnResultFromTryGetWhenThereIsPermissionsClaims(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        OperationPermissionClaimClaimsProvider sut,
        long? accountLegalEntityId,
        Operation operation)
    {
        const bool hasPermission = true;
        
        var claimsPrinciple = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ProviderClaims.Name, "TEST"),
                new Claim(ProviderClaims.OperationPermissions, JsonConvert.SerializeObject(new List<OperationPermission>()
                {
                    new()
                    {
                        AccountLegalEntityId = accountLegalEntityId,
                        Operation = operation,
                        HasPermission = hasPermission,
                    }
                }))
            })
        });

        var httpContext = new DefaultHttpContext(new FeatureCollection()) { User = claimsPrinciple };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        
        var actual = sut.TryGetPermission(accountLegalEntityId, operation, out var result);

        using (new AssertionScope())
        {
            actual.Should().BeTrue();
            result.Should().BeTrue();
        }
    }
}