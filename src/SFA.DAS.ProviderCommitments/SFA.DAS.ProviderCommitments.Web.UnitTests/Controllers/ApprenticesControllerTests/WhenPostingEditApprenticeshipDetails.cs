using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingEditApprenticeshipDetails
    {
        private Fixture _autoFixture;
        private WhenPostingEditApprenticeshipDetailsFixture _fixture;
        private EditApprenticeshipRequestViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();
            _fixture = new WhenPostingEditApprenticeshipDetailsFixture();
            _viewModel = new EditApprenticeshipRequestViewModel();
        }

        [Test]
        public async Task VerifyValidationApiIsCalled()
        {
            await _fixture.EditApprenticeship(_viewModel);
            _fixture.VerifyValidationApiIsCalled();
        }

        [Test]
        public async Task VerifyMapperIsCalled()
        {
            await _fixture.EditApprenticeship(_viewModel);
            _fixture.VerifyMapperIsCalled();
        }

        [Test]
        public async Task VerifyRedirectedToConfirmEditApprenticeship()
        {
            var result = await _fixture.EditApprenticeship(_viewModel);
            _fixture.VerifyRedirectedToAction(result);
        }
    }

    public class WhenPostingEditApprenticeshipDetailsFixture : ApprenticeControllerTestFixtureBase
    {
        public WhenPostingEditApprenticeshipDetailsFixture() : base()
        {
            _controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
        }

        public async Task<IActionResult> EditApprenticeship(EditApprenticeshipRequestViewModel viewModel)
        {
            return await _controller.EditApprenticeship(viewModel);
        }

        public void VerifyValidationApiIsCalled()
        {
            _mockCommitmentsApiClient.Verify(x => x.ValidateApprenticeshipForEdit(It.IsAny<ValidateApprenticeshipForEditRequest>(), CancellationToken.None), Times.Once());
        }

        public void VerifyMapperIsCalled()
        {
            _mockMapper.Verify(x => x.Map<ValidateApprenticeshipForEditRequest>(It.IsAny<EditApprenticeshipRequestViewModel>()), Times.Once());
        }

        public void VerifyRedirectedToAction(IActionResult actionResult)
        {
            actionResult.VerifyReturnsRedirectToActionResult().WithActionName("ConfirmEditApprenticeship");
        }
    }
}
