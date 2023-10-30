using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers
{
    public class ProviderAccountControllerTest
    {
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
                Assert.IsNotNull(actual);
                actual.Url.Should().Be(redirectUrl);
            }
        }
    }
}
