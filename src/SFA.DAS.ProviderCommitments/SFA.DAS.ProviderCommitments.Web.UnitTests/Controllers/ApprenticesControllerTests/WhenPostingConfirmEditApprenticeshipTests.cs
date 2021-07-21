using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingConfirmEditApprenticeshipTests
    {
        private const string EditApprenticeNeedReapproval = "Change saved and sent to employer to approve";
        private const string EditApprenticeshipUpdated = "Change saved (re-approval not required)";
        private WhenPostingConfirmEditApprenticeshipTestsFixture fixture;

        [SetUp]
        public void Arrange()
        {
            fixture = new WhenPostingConfirmEditApprenticeshipTestsFixture();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_WithIntermediateUpdate_ThenRedirectToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            viewModel.ConfirmChanges = true;
            var result = await fixture.ConfirmEditApprenticeship(viewModel);

            fixture.VerifyRedirectToDetails(viewModel, result);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_WithIntermediateUpdate_WithCorrectFlashMessage(ConfirmEditApprenticeshipViewModel viewModel)
        {
            viewModel.ConfirmChanges = true;
            await fixture.ConfirmEditApprenticeship(viewModel);

            fixture.ConfirmFlashMessageLevel();
            fixture.ConfirmFlashMessageValue(EditApprenticeNeedReapproval);
        }

        [Test, MoqAutoData]
        public async Task AndTheApprenticeship_IsEdit_WithImmediateUpdate_ThenRedirectToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            fixture.SetNeedReapproval(false);

            var result = await fixture.ConfirmEditApprenticeship(viewModel);

            fixture.VerifyRedirectToDetails(viewModel, result);
        }

        [Test, MoqAutoData]
        public async Task AndTheApprenticeship_IsEdit_WithImmediateUpdate_WithCorrectFlashMessage(ConfirmEditApprenticeshipViewModel viewModel)
        {
            fixture.SetNeedReapproval(false);

            await fixture.ConfirmEditApprenticeship(viewModel);

            fixture.ConfirmFlashMessageLevel();
            fixture.ConfirmFlashMessageValue(EditApprenticeshipUpdated);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_CommitmentApi_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            fixture.VerifyEditApprenticeshipApiCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_CommitmentApi_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            fixture.VerifyEditApprenticeshipApiNeverCalled();
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = true;

            //Act
            await fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            fixture.EditApprenticeshipApiRequestMapperIsCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsConfirmed_and_Mapper_IsNotCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            await fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            fixture.EditApprenticeshipApiRequestMapperIsNeverCalled(viewModel);
        }

        [Test, MoqAutoData]
        public async Task AndTheEditApprenticeship_IsNotConfirmed_ThenRedirectsToApprenticeshipDetails(ConfirmEditApprenticeshipViewModel viewModel)
        {
            //Arrange
            viewModel.ConfirmChanges = false;

            //Act
            var result = await fixture.ConfirmEditApprenticeship(viewModel);

            //Assert
            fixture.VerifyRedirectToDetails(viewModel, result);
        }
    }

    public class WhenPostingConfirmEditApprenticeshipTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private const string FlashMessage = "FlashMessage";
        private const string FlashMessageLevel = "FlashMessageLevel";
        private EditApprenticeshipResponse apiResponse;
        private EditApprenticeshipApiRequest apiRequest;
        private const string InfoLevel = "Info";

        public WhenPostingConfirmEditApprenticeshipTestsFixture()
        {
            apiRequest = new EditApprenticeshipApiRequest();
          //  viewModel = new ConfirmEditApprenticeshipViewModel();
            apiResponse = new EditApprenticeshipResponse { NeedReapproval = true };
            SetUpMockCommitmentClientApi();
            SetupModelMapper();
            _controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);
        }

        private void SetUpMockCommitmentClientApi()
        {
            _mockCommitmentsApiClient.Setup(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(apiResponse));
        }

        private void SetupModelMapper()
        {
            _mockMapper.Setup(x => x.Map<EditApprenticeshipApiRequest>(It.IsAny<ConfirmEditApprenticeshipViewModel>())).ReturnsAsync(apiRequest);
        }

        public Task<IActionResult> ConfirmEditApprenticeship(ConfirmEditApprenticeshipViewModel viewModel)
        {
           return _controller.ConfirmEditApprenticeship(viewModel);
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
            _controller.TempData.TryGetValue(key, out valueFromTempData);
            Assert.AreEqual(valueFromTempData.ToString(), value);
        }

        internal void VerifyRedirectToDetails(ConfirmEditApprenticeshipViewModel viewModel, IActionResult result)
        {
            var redirect = result.VerifyReturnsRedirectToActionResult();
            Assert.AreEqual(redirect.ActionName, "Details");
            Assert.AreEqual(redirect.RouteValues["ProviderId"], viewModel.ProviderId);
            Assert.AreEqual(redirect.RouteValues["ApprenticeshipHashedId"], viewModel.ApprenticeshipHashedId);
        }

        internal void SetNeedReapproval(bool value)
        {
            apiResponse.NeedReapproval = value;
        }

        internal void VerifyEditApprenticeshipApiCalled()
        {
            _mockCommitmentsApiClient.Verify(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), CancellationToken.None), Times.Once);
        }

        internal void VerifyEditApprenticeshipApiNeverCalled()
        {
            _mockCommitmentsApiClient.Verify(x => x.EditApprenticeship(It.IsAny<EditApprenticeshipApiRequest>(), CancellationToken.None), Times.Never);
        }

        internal void EditApprenticeshipApiRequestMapperIsCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            _mockMapper.Verify(x => x.Map<EditApprenticeshipApiRequest>(viewModel), Times.Once);
        }

        internal void EditApprenticeshipApiRequestMapperIsNeverCalled(ConfirmEditApprenticeshipViewModel viewModel)
        {
            _mockMapper.Verify(x => x.Map<EditApprenticeshipApiRequest>(viewModel), Times.Never);
        }
    }
}
