using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

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

            await fixture.ActOnAddApprenticeship();

            fixture.VerifyMapperWasCalled();
        }
    }

    public class WhenAddingACohortWithDraftApprenticeFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly CreateCohortWithDraftApprenticeshipRequest _request;
        private readonly Mock<ITempDataDictionary> _tempData;
        private object _viewModelAsString;

        public WhenAddingACohortWithDraftApprenticeFixture()
        {
            _request = new CreateCohortWithDraftApprenticeshipRequest
            {
                DeliveryModel = DeliveryModel.PortableFlexiJob,
                CourseCode = "ABC123"
            };

            var redirectModel = new CreateCohortRedirectModel { RedirectTo = CreateCohortRedirectModel.RedirectTarget.SelectCourse };
            var viewModel = new AddDraftApprenticeshipViewModel
            {
                DeliveryModel = DeliveryModel.Regular,
                CourseCode = "DIFF123"
            };
            _viewModelAsString = JsonConvert.SerializeObject(viewModel);
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(viewModel);
            _modelMapper.Setup(x => x.Map<CreateCohortRedirectModel>(_request)).ReturnsAsync(redirectModel);

            _sut = new CohortController(
                Mock.Of<IMediator>(),
                _modelMapper.Object, 
                Mock.Of<ILinkGenerator>(), 
                Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IEncodingService>(),
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthorizationService>()
                );

            _tempData = new Mock<ITempDataDictionary>();
            _sut.TempData = _tempData.Object;

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

        public IActionResult Act() => _sut.AddNewDraftApprenticeship(_request).Result;
        public async Task<IActionResult> ActOnAddApprenticeship() => await _sut.AddDraftApprenticeship(_request);
    }
}