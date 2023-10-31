using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadValidationErrors
    {
        [Test]
        public async Task PostFileUploadStartViewModel_ShouldRedirectToCohorts()
        {
            var fixture = new PostFileUploadValidationErrorsFixture();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadReview"); ;
        }

        [Test]
        public async Task PostFileUploadStartViewModel_CreateCsvRecordsInCache()
        {
            var fixture = new PostFileUploadValidationErrorsFixture();

            await fixture.Act();
            fixture.VerifyCsvRecordCached();
        }
    }

    public class PostFileUploadValidationErrorsFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly FileUploadValidateViewModel _viewModel;

        public PostFileUploadValidationErrorsFixture()
        {
            var fixture = new Fixture();
            _viewModel = fixture.Build<FileUploadValidateViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            var request = fixture.Create<FileUploadReviewRequest>();
            var outerApiService = new Mock<IOuterApiService>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<FileUploadReviewRequest>(_viewModel)).ReturnsAsync(() => request);

            var mediator = new Mock<IMediator>();

            _sut = new CohortController(mediator.Object, _mockModelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), outerApiService.Object,Mock.Of<IAuthorizationService>());
            
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _sut.TempData = tempData;
        }

        public PostFileUploadValidationErrorsFixture VerifyCsvRecordCached()
        {
            _mockModelMapper.Verify(x => x.Map<FileUploadReviewRequest>(_viewModel), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await _sut.FileUploadValidationErrors(_viewModel);
    }
}
