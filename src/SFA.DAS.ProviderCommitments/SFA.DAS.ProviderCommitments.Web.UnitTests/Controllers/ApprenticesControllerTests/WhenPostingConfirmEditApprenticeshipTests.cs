using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingConfirmEditApprenticeshipTests
    {
        private const string EditApprenticeNeedReapproval = "Change saved and sent to employer to approve";
        private const string EditApprenticeshipUpdated = "Change saved (re-approval not required)";
        private WhenPostingConfirmEditApprenticeshipTestsFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingConfirmEditApprenticeshipTestsFixture();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_WithIntermediateUpdate_ThenRedirectToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            viewModel.ConfirmChanges = true;
            var result = await _fixture.ConfirmEditApprenticeship(viewModel);

            WhenPostingConfirmEditApprenticeshipTestsFixture.VerifyRedirectToDetails(viewModel, result);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_WithIntermediateUpdate_WithCorrectFlashMessage(ConfirmEditApprenticeshipViewModel viewModel)
        {
            viewModel.ConfirmChanges = true;
            await _fixture.ConfirmEditApprenticeship(viewModel);

            _fixture.ConfirmFlashMessageLevel();
            _fixture.ConfirmFlashMessageValue(EditApprenticeNeedReapproval);
        }

        [Test, MoqAutoData]
        public async Task AndTheApprenticeship_IsEdit_WithImmediateUpdate_ThenRedirectToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            _fixture.SetNeedReapproval(false);

            var result = await _fixture.ConfirmEditApprenticeship(viewModel);

            WhenPostingConfirmEditApprenticeshipTestsFixture.VerifyRedirectToDetails(viewModel, result);
        }

        [Test, MoqAutoData]
        public async Task AndTheApprenticeship_IsEdit_WithImmediateUpdate_WithCorrectFlashMessage(ConfirmEditApprenticeshipViewModel viewModel)
        {
            _fixture.SetNeedReapproval(false);

            await _fixture.ConfirmEditApprenticeship(viewModel);

            _fixture.ConfirmFlashMessageLevel();
            _fixture.ConfirmFlashMessageValue(EditApprenticeshipUpdated);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_OuterApi_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.VerifyConfirmEditApprenticeshipApiCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_OuterApi_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.VerifyConfirmEditApprenticeshipApiNeverCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.ConfirmEditApprenticeshipRequestMapperIsCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.ConfirmEditApprenticeshipRequestMapperIsNeverCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_ThenRedirectsToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            var result = await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            WhenPostingConfirmEditApprenticeshipTestsFixture.VerifyRedirectToDetails(viewModel, result);
        }
    }

    public class WhenPostingConfirmEditApprenticeshipTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private const string FlashMessage = "FlashMessage";
        private const string FlashMessageLevel = "FlashMessageLevel";
        private readonly ConfirmEditApprenticeshipResponse _apiResponse;
        private readonly ConfirmEditApprenticeshipRequest _apiRequest;
        private const string InfoLevel = "Info";

        public WhenPostingConfirmEditApprenticeshipTestsFixture()
        {
            _apiRequest = new ConfirmEditApprenticeshipRequest();
            _apiResponse = new ConfirmEditApprenticeshipResponse { NeedReapproval = true };
            SetUpMockOuterApiService();
            SetupModelMapper();
            Controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);
        }

        private void SetUpMockOuterApiService()
        {
            MockOuterApiService.Setup(x => x.ConfirmEditApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ConfirmEditApprenticeshipRequest>())).Returns(() => Task.FromResult(_apiResponse));
        }

        private void SetupModelMapper()
        {
            MockMapper.Setup(x => x.Map<ConfirmEditApprenticeshipRequest>(It.IsAny<ConfirmEditApprenticeshipViewModel>())).ReturnsAsync(_apiRequest);
        }

        public Task<IActionResult> ConfirmEditApprenticeship(ConfirmEditApprenticeshipViewModel viewModel)
        {
            return Controller.ConfirmEditApprenticeship(viewModel);
        }

        internal void ConfirmFlashMessageValue(string editApprenticeNeedReapproval)
        {
            VerifyTempData(FlashMessage, editApprenticeNeedReapproval);
        }

        internal void ConfirmFlashMessageLevel()
        {
            VerifyTempData(FlashMessageLevel, InfoLevel);
        }

        private void VerifyTempData(string key, string value)
        {
            object valueFromTempData;
            Controller.TempData.TryGetValue(key, out valueFromTempData);
            value.Should().Be(valueFromTempData.ToString());
        }

        internal static void VerifyRedirectToDetails(ConfirmEditApprenticeshipViewModel viewModel, IActionResult result)
        {
            var redirect = result.VerifyReturnsRedirectToActionResult();
            using (new AssertionScope())
            {
                redirect.ActionName.Should().Be("Details");
                viewModel.ProviderId.Should().Be(long.Parse(redirect.RouteValues["ProviderId"].ToString()));
                viewModel.ApprenticeshipHashedId.Should().Be(redirect.RouteValues["ApprenticeshipHashedId"].ToString());
            }
        }

        internal void SetNeedReapproval(bool value)
        {
            _apiResponse.NeedReapproval = value;
        }

        internal void VerifyConfirmEditApprenticeshipApiCalled()
        {
            MockOuterApiService.Verify(x => x.ConfirmEditApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ConfirmEditApprenticeshipRequest>()), Times.Once);
        }

        internal void VerifyConfirmEditApprenticeshipApiNeverCalled()
        {
            MockOuterApiService.Verify(x => x.ConfirmEditApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ConfirmEditApprenticeshipRequest>()), Times.Never);
        }

        internal void ConfirmEditApprenticeshipRequestMapperIsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            MockMapper.Verify(x => x.Map<ConfirmEditApprenticeshipRequest>(viewModel), Times.Once);
        }

        internal void ConfirmEditApprenticeshipRequestMapperIsNeverCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            MockMapper.Verify(x => x.Map<ConfirmEditApprenticeshipRequest>(viewModel), Times.Never);
        }
    }
}