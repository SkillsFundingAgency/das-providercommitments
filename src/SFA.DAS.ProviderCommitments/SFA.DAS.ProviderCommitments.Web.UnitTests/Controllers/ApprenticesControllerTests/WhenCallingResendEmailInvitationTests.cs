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
        private WhenCallingResendEmailInvitationTestsFixture _fixture;

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
        private Mock<IAuthenticationService> AuthenticationService { get; }

        public WhenCallingResendEmailInvitationTestsFixture()
        {
            _request = AutoFixture.Create<ResendEmailInvitationRequest>();

            var userInfo = new Fixture().Create<UserInfo>();
            AuthenticationService = new Mock<IAuthenticationService>();
            AuthenticationService.Setup(x => x.UserInfo).Returns(userInfo);

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

            Assert.AreEqual(redirect.RouteValues["ProviderId"], _request.ProviderId);
            Assert.AreEqual(redirect.RouteValues["ApprenticeshipHashedId"], _request.ApprenticeshipHashedId);
        }
    }
}
