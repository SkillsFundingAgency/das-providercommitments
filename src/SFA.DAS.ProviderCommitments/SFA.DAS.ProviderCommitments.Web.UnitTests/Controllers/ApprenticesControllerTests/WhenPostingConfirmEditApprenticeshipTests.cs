using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
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

            _fixture.VerifyRedirectToDetails(viewModel, result);
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

            _fixture.VerifyRedirectToDetails(viewModel, result);
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
        public async Task AndTheEditApprenticeship_IsConfirmed_CommitmentApi_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.VerifyEditApprenticeshipApiCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_CommitmentApi_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.VerifyEditApprenticeshipApiNeverCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.EditApprenticeshipApiRequestMapperIsCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.EditApprenticeshipApiRequestMapperIsNeverCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_ThenRedirectsToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            var result = await _fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            _fixture.VerifyRedirectToDetails(viewModel, result);
        }
    }

    public class WhenPostingConfirmEditApprenticeshipTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private const string FlashMessage = "FlashMessage";
        private const string FlashMessageLevel = "FlashMessageLevel";
        private readonly EditApprenticeshipResponse _apiResponse;
        private readonly EditApprenticeshipApiRequest _apiRequest;
        private const string InfoLevel = "Info";

        public WhenPostingConfirmEditApprenticeshipTestsFixture()
        {
            _apiRequest = new EditApprenticeshipApiRequest();
            _apiResponse = new EditApprenticeshipResponse { NeedReapproval = true };
            SetUpMockCommitmentClientApi();
            SetupModelMapper();
            Controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);
        }

        private void SetUpMockCommitmentClientApi()
        {
            MockCommitmentsApiClient.Setup(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(_apiResponse));
        }

        private void SetupModelMapper()
        {
            MockMapper.Setup(x => x.Map<EditApprenticeshipApiRequest>(It.IsAny<ConfirmEditApprenticeshipViewModel>())).ReturnsAsync(_apiRequest);
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
            Assert.That(value, Is.EqualTo(valueFromTempData.ToString()));
        }

        internal void VerifyRedirectToDetails(ConfirmEditApprenticeshipViewModel viewModel, IActionResult result)
        {
            var redirect = result.VerifyReturnsRedirectToActionResult();
            Assert.That("Details", Is.EqualTo(redirect.ActionName));
            Assert.That(viewModel.ProviderId, Is.EqualTo(redirect.RouteValues["ProviderId"]));
            Assert.That(viewModel.ApprenticeshipHashedId, Is.EqualTo(redirect.RouteValues["ApprenticeshipHashedId"]));
        }

        internal void SetNeedReapproval(bool value)
        {
            _apiResponse.NeedReapproval = value;
        }

        internal void VerifyEditApprenticeshipApiCalled()
        {
            MockCommitmentsApiClient.Verify(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), CancellationToken.None), Times.Once);
        }

        internal void VerifyEditApprenticeshipApiNeverCalled()
        {
            MockCommitmentsApiClient.Verify(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), CancellationToken.None), Times.Never);
        }

        internal void EditApprenticeshipApiRequestMapperIsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            MockMapper.Verify(x => x.Map<EditApprenticeshipApiRequest>(viewModel), Times.Once);
        }

        internal void EditApprenticeshipApiRequestMapperIsNeverCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            MockMapper.Verify(x => x.Map<EditApprenticeshipApiRequest>(viewModel), Times.Never);
        }
    }
}
