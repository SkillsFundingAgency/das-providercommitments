﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.Encoding;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadValidationErrors
    {
        [Test]
        public async Task PostFileUploadStartViewModel_ShouldRedirectToCohorts()
        {
            var fixture = new PostFileUploadValidationErrorsixture();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadReview"); ;
         
        }

        [Test]
        public async Task PostFileUploadStartViewModel_CreateCsvRecordsInCache()
        {
            var fixture = new PostFileUploadValidationErrorsixture();

            await fixture.Act();
            fixture.VerifyCsvRecordCached();
        }
    }

    public class PostFileUploadValidationErrorsixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly FileUploadValidateViewModel _viewModel;
        private readonly FileUploadReviewRequest _request;
        private readonly Mock<IMediator> _mediator;

        public PostFileUploadValidationErrorsixture()
        {
            var fixture = new Fixture();
            _viewModel = fixture.Build<FileUploadValidateViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            _request = fixture.Create<FileUploadReviewRequest>();
            _outerApiService = new Mock<IOuterApiService>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<FileUploadReviewRequest>(_viewModel)).ReturnsAsync(() => _request);

            _mediator = new Mock<IMediator>();

            Sut = new CohortController(_mediator.Object, _mockModelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), _outerApiService.Object,Mock.Of<IAuthorizationService>());
            
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            Sut.TempData = tempData;
        }

        public PostFileUploadValidationErrorsixture VerifyCsvRecordCached()
        {
            _mockModelMapper.Verify(x => x.Map<FileUploadReviewRequest>(_viewModel), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await Sut.FileUploadValidationErrors(_viewModel);
    }
}
