using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.Encoding;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using Microsoft.AspNetCore.Http;

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
            result.VerifyReturnsRedirectToActionResult().WithActionName("Cohorts"); ;
         
        }

        [Test]
        public async Task PostFileUploadStartViewModel_ShouldBulkUploadApprenticeships()
        {
            var fixture = new PostFileUploadStartFixture();

            await fixture.Act();
            fixture.VerifyBulkApprenticeshipUploaded();
        }
    }

    public class PostFileUploadStartFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClient;
        private readonly FileUploadStartViewModel _viewModel;
        private readonly BulkUploadAddDraftApprenticeshipsRequest _apiRequest;

        public PostFileUploadStartFixture()
        {
            var fixture = new Fixture();
            _viewModel = fixture.Build<FileUploadStartViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            _apiRequest = fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(_viewModel)).ReturnsAsync(() => _apiRequest);

            _commitmentApiClient
                .Setup(x => x.BulkUploadDraftApprenticeships(It.IsAny<long>(), It.IsAny<BulkUploadAddDraftApprenticeshipsRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            Sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, Mock.Of<ILinkGenerator>(), _commitmentApiClient.Object, Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public PostFileUploadStartFixture VerifyBulkApprenticeshipUploaded()
        {
            _commitmentApiClient.Verify(x => x.BulkUploadDraftApprenticeships(It.IsAny<long>(), _apiRequest, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await Sut.FileUploadStart(_viewModel);
    }
}
