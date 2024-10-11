using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadReviewDelete
    {
        [TestCase(null)]
        [TestCase(FileUploadReviewDeleteRedirect.UploadAgain)]
        public async Task Then_Redirects_To_FileUploadStart(FileUploadReviewDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            var result =  await fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadStart");
        }


        [TestCase(FileUploadReviewDeleteRedirect.Home)]
        public async Task Then_Redirects_To_Home(FileUploadReviewDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            var result = await fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirect().WithUrl("pasurl/account");
        }

        [Test]
        public async Task Then_Cache_Is_Cleared()
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            await fixture.Act();

            fixture.VerifyCacheIsCleared();
        }
    }

    public class WhenGettingFileUploadReviewDeleteFixture
    {
        private readonly CohortController _sut;
        private readonly FileUploadReviewDeleteRequest _request;
        private const long ProviderId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IMediator> _mediator;

        public WhenGettingFileUploadReviewDeleteFixture()
        {
            var fixture = new Fixture();

            var viewModel = fixture.Create<FileUploadReviewViewModel>();
            _request = new FileUploadReviewDeleteRequest { ProviderId = ProviderId, CacheRequestId = _cacheRequestId };

            var modelMapper = new Mock<IModelMapper>();
            modelMapper.Setup(x => x.Map<FileUploadReviewViewModel>(_request)).ReturnsAsync(viewModel);
            
            _mediator = new Mock<IMediator>();
            
           var linkGenerator = new Mock<ILinkGenerator>();
            linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink("/account")).Returns("pasurl/account");
        
            _sut = new CohortController(_mediator.Object, modelMapper.Object, linkGenerator.Object, Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReviewDelete(_request);

        internal WhenGettingFileUploadReviewDeleteFixture WithRedirectTo(FileUploadReviewDeleteRedirect? redirectTo)
        {
            _request.RedirectTo = redirectTo;
            return this;
        }

        internal void VerifyCacheIsCleared()
        {
            _mediator.Verify(x => x.Send(It.IsAny<DeleteCachedFileCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}