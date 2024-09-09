using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingChangeOption
    {
        private ChangeOptionViewModel _viewModel;

        private WhenPostingChangeOptionFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();

            _viewModel = autoFixture.Create<ChangeOptionViewModel>();

            _fixture = new WhenPostingChangeOptionFixture();
        }

        [Test]
        public async Task VerifyMapperIsCalled()
        {
            await _fixture.ChangeOption(_viewModel);

            _fixture.VerifyMapperIsCalled();
        }

        [Test]
        public async Task VerifyTempDataStored()
        {
            await _fixture.ChangeOption(_viewModel);

            _fixture.VerifyTempDataStored();
        }

        [Test]
        public async Task VerifyRedirectToConfirmEditApprenticeship()
        {
            var result = await _fixture.ChangeOption(_viewModel);

            WhenPostingChangeOptionFixture.VerifyReturnToConfirmEditApprenticeship(result as RedirectToActionResult);
        }
    }

    public class WhenPostingChangeOptionFixture : ApprenticeControllerTestFixtureBase
    {
        public WhenPostingChangeOptionFixture()
        {
            Controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
        }

        public async Task<IActionResult> ChangeOption(ChangeOptionViewModel viewModel)
        {
            return await Controller.ChangeOption(viewModel);
        }

        public void VerifyMapperIsCalled()
        {
            MockMapper.Verify(x => x.Map<EditApprenticeshipRequestViewModel>(It.IsAny<ChangeOptionViewModel>()), Times.Once());
        }

        public void VerifyTempDataStored()
        {
            Controller.TempData.ContainsKey("EditApprenticeshipRequestViewModel").Should().BeTrue();
        }

        public static void VerifyReturnToConfirmEditApprenticeship(RedirectToActionResult redirectResult)
        {
            redirectResult.ActionName.Should().Be("ConfirmEditApprenticeship");
        }
    }
}
