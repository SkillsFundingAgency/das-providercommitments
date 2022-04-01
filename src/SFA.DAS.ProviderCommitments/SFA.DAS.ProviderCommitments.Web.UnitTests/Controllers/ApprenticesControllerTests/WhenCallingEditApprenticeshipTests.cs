using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Types;

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

        [Test]
        public async Task AndWeHaveAnExistingEditViewModel_ThenTheTempModelIsPassedToTheView()
        {
            var result = await _fixture.WithTempModel().EditApprenticeship();
            _fixture.VerifyViewModelIsEquivalentToTempViewModel(result as ViewResult);
        }
    }

    public class WhenCallingEditApprenticeshipTestsFixture : ApprenticeControllerTestFixtureBase
    {
        private readonly EditApprenticeshipRequest _request;
        private readonly EditApprenticeshipRequestViewModel _viewModel;
        private readonly EditApprenticeshipRequestViewModel _tempViewModel;
        private object _tempViewModelAsString;


        public WhenCallingEditApprenticeshipTestsFixture() : base()
        {
            _request = _autoFixture.Create<EditApprenticeshipRequest>();
            _viewModel = _autoFixture.Create<EditApprenticeshipRequestViewModel>();
            _tempViewModel = _autoFixture.Create<EditApprenticeshipRequestViewModel>();

            _tempViewModelAsString = JsonConvert.SerializeObject(_tempViewModel);

            _mockMapper.Setup(m => m.Map<EditApprenticeshipRequestViewModel>(_request))
                .ReturnsAsync(_viewModel);
        }

        public async Task<IActionResult> EditApprenticeship()
        {
            return await _controller.EditApprenticeship(_request);
        }

        public WhenCallingEditApprenticeshipTestsFixture SetFlexibleDeliveryModel()
        {
            _viewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;
            return this;
        }

        public WhenCallingEditApprenticeshipTestsFixture WithTempModel()
        {
            _mockTempData.Setup(x => x.TryGetValue("ViewModelForEdit", out _tempViewModelAsString));
            _viewModel.DeliveryModel = DeliveryModel.PortableFlexiJob;
            return this;
        }

        public void VerifyViewModel(ViewResult viewResult)
        {
            var viewModel = viewResult.Model as EditApprenticeshipRequestViewModel;

            Assert.IsInstanceOf<EditApprenticeshipRequestViewModel>(viewModel);
            Assert.AreEqual(_viewModel, viewModel);
        }

        public void VerifyViewModelIsEquivalentToTempViewModel(ViewResult viewResult)
        {
            var viewModel = viewResult.Model as EditApprenticeshipRequestViewModel;

            Assert.IsInstanceOf<EditApprenticeshipRequestViewModel>(viewModel);
            _tempViewModel.Should().BeEquivalentTo(viewModel);
        }

    }
}
