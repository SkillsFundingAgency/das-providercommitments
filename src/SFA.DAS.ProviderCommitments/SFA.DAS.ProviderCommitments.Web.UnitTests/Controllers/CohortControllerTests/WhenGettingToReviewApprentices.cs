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
    public class WhenGettingToReviewApprentices
    {
        [Test]
        public async Task ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingToReviewApprenticesFixture();

            //Act
            var result = await fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_ReviewApprenticeViewModel()
        {
            //Arrange
            var fixture = new WhenGettingToReviewApprenticesFixture();

            //Act
            var viewResult = await fixture.Act();

            //Assert
            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadReviewApprenticeViewModel>();
            model.Should().NotBeNull();
        }
    }

    public class WhenGettingToReviewApprenticesFixture
    {
        private readonly CohortController _sut;        
        private readonly FileUploadReviewApprenticeRequest _request;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        
        private const long ProviderId = 123;
        private const string CohortRef = "VLB8N4";
        
        public WhenGettingToReviewApprenticesFixture()
        {
            var fixture = new Fixture();

            var viewModel = fixture.Create<FileUploadReviewApprenticeViewModel>();
            _request = new FileUploadReviewApprenticeRequest { ProviderId = ProviderId, CacheRequestId = _cacheRequestId, CohortRef = CohortRef };

            var modelMapper = new Mock<IModelMapper>();
            modelMapper.Setup(x => x.Map<FileUploadReviewApprenticeViewModel>(_request)).ReturnsAsync(viewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReviewApprentices(_request);
    }
}
