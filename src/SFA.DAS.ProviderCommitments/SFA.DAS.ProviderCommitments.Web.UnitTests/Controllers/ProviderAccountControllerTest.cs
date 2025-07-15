using System.Collections.Generic;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System.Security.Claims;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers;

public class ProviderAccountControllerTest
{
    [Test, MoqAutoData]
    public void Index_WhenUkprnClaimExists_RedirectsToCohort(
        string ukprn,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Greedy] ProviderAccountController controller)
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ProviderClaims.Ukprn, ukprn)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = controller.Index(httpContextAccessor.Object) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.Cohort);
        result.RouteValues["providerId"].Should().Be(ukprn);
    }

    public class WhenGettingTheDashboard
    {
        [Test, MoqAutoData]
        public void Then_Returned_Redirected_To_SharedUiDashboard(
            string redirectUrl,
            [Frozen] ProviderSharedUIConfiguration mockOptions,
            [Greedy] ProviderAccountController controller)
        {
            //Arrange
            mockOptions.DashboardUrl = redirectUrl;

            //Act
            var actual = controller.Dashboard() as RedirectResult;

            //Assert
            actual.Should().NotBeNull();
            actual.Url.Should().Be(redirectUrl);
        }
    }
}