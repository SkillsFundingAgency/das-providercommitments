using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.Encoding;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadStart
    {
        [Test]
        public async Task PostFileUploadStartViewModel_ShouldRedirectToCohorts()
        {
            var fixture = new PostFileUploadStartFixture();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadReview"); ;
         
        }

        [Test]
        public async Task PostFileUploadStartViewModel_CreateCsvRecordsInCache()
        {
            var fixture = new PostFileUploadStartFixture();

            await fixture.Act();
            fixture.VerifyCsvRecordCached();
        }
    }

    public class PostFileUploadStartFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClient;
        private readonly FileUploadStartViewModel _viewModel;
        private readonly FileUploadReviewRequest _request;
        private readonly Mock<IMediator> _mediator;

        public PostFileUploadStartFixture()
        {
            var fixture = new Fixture();
            _viewModel = fixture.Build<FileUploadStartViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            _request = fixture.Create<FileUploadReviewRequest>();
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<FileUploadReviewRequest>(_viewModel)).ReturnsAsync(() => _request);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<FileUploadValidateDataRequest>(), CancellationToken.None)).ReturnsAsync(() => new BulkUploadValidateApiResponse { BulkUploadValidationErrors = new List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError>() });

            Sut = new CohortController(_mediator.Object, _mockModelMapper.Object, Mock.Of<ILinkGenerator>(), _commitmentApiClient.Object, Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            Sut.TempData = tempData;
        }

        public PostFileUploadStartFixture VerifyCsvRecordCached()
        {
            _mockModelMapper.Verify(x => x.Map<FileUploadReviewRequest>(_viewModel), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await Sut.FileUploadStart(_viewModel);
    }
}
