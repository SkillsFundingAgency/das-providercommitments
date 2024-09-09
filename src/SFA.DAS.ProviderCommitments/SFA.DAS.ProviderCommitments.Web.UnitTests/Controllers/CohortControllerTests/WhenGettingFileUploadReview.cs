using System;
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
    public class WhenGettingFileUploadReview
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadReviewFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_FileUploadReviewViewModel()
        {
            var fixture = new WhenGettingFileUploadReviewFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadReviewViewModel>();

            Assert.That(model, Is.Not.Null);
        }
    }

    public class WhenGettingFileUploadReviewFixture
    {
        private readonly CohortController _sut;
        private readonly FileUploadReviewRequest _request;
        private const long ProviderId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();

        public WhenGettingFileUploadReviewFixture()
        {
            var fixture = new Fixture();
            
            var viewModel = fixture.Create<FileUploadReviewViewModel>();
            _request = new FileUploadReviewRequest { ProviderId = ProviderId, CacheRequestId = _cacheRequestId };

            var modelMapper = new Mock<IModelMapper>();
            modelMapper.Setup(x => x.Map<FileUploadReviewViewModel>(_request)).ReturnsAsync(viewModel);
        
            _sut = new CohortController(Mock.Of<IMediator>(), modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReview(_request);
    }
}