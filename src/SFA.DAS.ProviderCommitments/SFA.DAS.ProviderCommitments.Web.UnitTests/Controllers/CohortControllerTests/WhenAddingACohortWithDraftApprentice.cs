using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenAddingACohortWithDraftApprentice
    {
        [Test]
        public void AndDeliveryModelToggleIsOn_ThenRedirectedToSelectCourse()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

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
    }

    public class WhenAddingACohortWithDraftApprenticeFixture
    {
        public CohortController Sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly AddDraftApprenticeshipViewModel _viewModel;
        private readonly CreateCohortRedirectModel _redirectModel;
        private readonly CreateCohortWithDraftApprenticeshipRequest _request;
        private readonly Mock<ITempDataDictionary> _tempData;
        private object _viewModelAsString;

        public WhenAddingACohortWithDraftApprenticeFixture()
        {
            _request = new CreateCohortWithDraftApprenticeshipRequest();
            _request.DeliveryModel = DeliveryModel.PortableFlexiJob;
            _request.CourseCode = "ABC123";

            _redirectModel = new CreateCohortRedirectModel { RedirectTo = CreateCohortRedirectModel.RedirectTarget.SelectCourse };
            _viewModel = new AddDraftApprenticeshipViewModel();
            _viewModel.DeliveryModel = DeliveryModel.Regular;
            _viewModel.CourseCode = "DIFF123";
            _viewModelAsString = JsonConvert.SerializeObject(_viewModel);
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(_viewModel);
            _modelMapper.Setup(x => x.Map<CreateCohortRedirectModel>(_request)).ReturnsAsync(_redirectModel);

            Sut = new CohortController(
                Mock.Of<IMediator>(),
                _modelMapper.Object, 
                Mock.Of<ILinkGenerator>(), 
                Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IEncodingService>(),
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthorizationService>(), 
                Mock.Of<ILogger<CohortController>>()
                );

            _tempData = new Mock<ITempDataDictionary>();
            Sut.TempData = _tempData.Object;

        }

        public WhenAddingACohortWithDraftApprenticeFixture WithTempDataSet()
        {
            _tempData.Setup(x => x.TryGetValue(nameof(AddDraftApprenticeshipViewModel), out _viewModelAsString));
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

        public IActionResult Act() => Sut.AddNewDraftApprenticeship(_request).Result;
        public async Task<IActionResult> ActOnAddApprenticeship() => await Sut.AddDraftApprenticeship(_request);
    }
}