using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenCallingResendEmailInvitationTests
    {
        WhenCallingResendEmailInvitationTestsFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenCallingResendEmailInvitationTestsFixture();
        }

        [Test]
        public async Task ThenTheCorrectViewIsReturned()
        {
            var result = await _fixture.ResendEmailInvitation();

            _fixture.VerifyResendApprenticeshipInvitationApiIsCalled();
            _fixture.VerifyRedirect(result);
        }
    }

    public class WhenCallingResendEmailInvitationTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private readonly ResendEmailInvitationRequest _request;
        public UserInfo UserInfo;
        public Mock<IAuthenticationService> AuthenticationService { get; }

        public WhenCallingResendEmailInvitationTestsFixture()
        {
            _request = AutoFixture.Create<ResendEmailInvitationRequest>();

            UserInfo = new Fixture().Create<UserInfo>();
            AuthenticationService = new Mock<IAuthenticationService>();
            AuthenticationService.Setup(x => x.UserInfo).Returns(UserInfo);

            Controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        public async Task<IActionResult> ResendEmailInvitation()
        {
            return await Controller.ResendEmailInvitation(AuthenticationService.Object, _request);
        }

        public void VerifyResendApprenticeshipInvitationApiIsCalled()
        {
            MockCommitmentsApiClient.Verify(x => x.ResendApprenticeshipInvitation(
                _request.ApprenticeshipId, It.Is<SaveDataRequest>(o => o.UserInfo != null), It.IsAny<CancellationToken>()), Times.Once());
        }

        public void VerifyRedirect(IActionResult result)
        {
            result.VerifyReturnsRedirectToActionResult().WithActionName("Details");

            var redirect = result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(_request.ProviderId, Is.EqualTo(redirect.RouteValues["ProviderId"]));
                Assert.That(_request.ApprenticeshipHashedId, Is.EqualTo(redirect.RouteValues["ApprenticeshipHashedId"]));
            });
        }
    }
}