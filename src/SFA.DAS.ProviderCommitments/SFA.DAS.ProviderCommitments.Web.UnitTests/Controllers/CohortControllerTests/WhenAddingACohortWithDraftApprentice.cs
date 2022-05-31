using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenAddingACohortWithDraftApprentice
    {
        [Test]
        public void ThenRedirectedToAddApprenticeship()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = fixture.Act() as RedirectToActionResult;

            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public void AndDeliveryModelToggleIsOn_ThenRedirectedToSelectCourse()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetDeliveryModelToggleOn();

            var result = fixture.Act() as RedirectToActionResult;

            result.ActionName.Should().Be("SelectCourse");
        }

        [Test]
        public async Task AndOnAddApprenticeshipPage_ThenReturnsView()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = await fixture.ActOnAddApprenticeship();

            result.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();
        }

        [Test]
        public async Task AndOnAddApprenticeshipPage_ThenMapperIsCalled()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = await fixture.ActOnAddApprenticeship();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task AndOnAddApprenticeshipPage_ThenTempDataIsRestoredAndNewValuesSet()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture().WithTempDataSet();

            var result = await fixture.ActOnAddApprenticeship();

            var model = result.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();

            fixture.VerifyDraftApprenticeshipWasRestoredAndValuesSet(model);
        }
    }

    public class WhenAddingACohortWithDraftApprenticeFixture
    {
        public CohortController Sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly Mock<IAuthorizationService> _providerFeatureToggle;
        private readonly AddDraftApprenticeshipViewModel _viewModel;
        private readonly CreateCohortWithDraftApprenticeshipRequest _request;
        private readonly Mock<ITempDataDictionary> _tempData;
        private object _viewModelAsString;

        public WhenAddingACohortWithDraftApprenticeFixture()
        {
            _request = new CreateCohortWithDraftApprenticeshipRequest();
            _request.DeliveryModel = DeliveryModel.PortableFlexiJob;
            _request.CourseCode = "ABC123";

            _viewModel = new AddDraftApprenticeshipViewModel();
            _viewModel.DeliveryModel = DeliveryModel.Regular;
            _viewModel.CourseCode = "DIFF123";
            _viewModelAsString = JsonConvert.SerializeObject(_viewModel);
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(_viewModel);
            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            Sut = new CohortController(
                Mock.Of<IMediator>(),
                _modelMapper.Object, 
                Mock.Of<ILinkGenerator>(), 
                Mock.Of<ICommitmentsApiClient>(),
                _providerFeatureToggle.Object,
                Mock.Of<IEncodingService>(),
                Mock.Of<IOuterApiService>(),
                Mock.Of<RecognitionOfPriorLearningConfiguration>());

            _tempData = new Mock<ITempDataDictionary>();
            Sut.TempData = _tempData.Object;

        }

        public WhenAddingACohortWithDraftApprenticeFixture WithTempDataSet()
        {
            _tempData.Setup(x => x.TryGetValue(nameof(AddDraftApprenticeshipViewModel), out _viewModelAsString));
            return this;
        }

        public WhenAddingACohortWithDraftApprenticeFixture SetDeliveryModelToggleOn()
        {
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.Is<string>(p => p == ProviderFeature.DeliveryModel))).Returns(true);
            return this;
        }

        public void VerifyDraftApprenticeshipWasRestoredAndValuesSet(AddDraftApprenticeshipViewModel viewModel)
        {
            if (viewModel == null)
            {
                Assert.Fail("View model has not been restored");
            }

            if (viewModel.DeliveryModel != _request.DeliveryModel || viewModel.CourseCode != _request.CourseCode)
            {
                Assert.Fail("View model does not have CourseCode and DeliveryModel set correctly");
            }
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_request));
        }

        public IActionResult Act() => Sut.AddNewDraftApprenticeship(_request);
        public async Task<IActionResult> ActOnAddApprenticeship() => await Sut.AddDraftApprenticeship(_request);
    }
}