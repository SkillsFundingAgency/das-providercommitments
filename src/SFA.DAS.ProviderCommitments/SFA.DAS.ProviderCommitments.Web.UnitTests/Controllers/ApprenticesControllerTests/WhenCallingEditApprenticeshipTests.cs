using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenCallingEditApprenticeshipTests
    {
        WhenCallingEditApprenticeshipTestsFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenCallingEditApprenticeshipTestsFixture();
        }

        [Test]
        public async Task ThenTheCorrectViewIsReturned()
        {
            var result = await _fixture.EditApprenticeship();

            _fixture.VerifyViewModel(result as ViewResult);
        }
    }

    public class WhenCallingEditApprenticeshipTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private readonly EditApprenticeshipRequest _request;
        private readonly EditApprenticeshipRequestViewModel _viewModel;

        public WhenCallingEditApprenticeshipTestsFixture() : base()
        {
            _request = _autoFixture.Create<EditApprenticeshipRequest>();
            _viewModel = _autoFixture.Create<EditApprenticeshipRequestViewModel>();


            _mockMapper.Setup(m => m.Map<EditApprenticeshipRequestViewModel>(_request))
                .ReturnsAsync(_viewModel);
        }

        public async Task<IActionResult> EditApprenticeship()
        {
            return await _controller.EditApprenticeship(_request);
        }

        public void VerifyViewModel(ViewResult viewResult)
        {
            var viewModel = viewResult.Model as EditApprenticeshipRequestViewModel;

            Assert.IsInstanceOf<EditApprenticeshipRequestViewModel>(viewModel);
            Assert.AreEqual(_viewModel, viewModel);
        }
    }
}
