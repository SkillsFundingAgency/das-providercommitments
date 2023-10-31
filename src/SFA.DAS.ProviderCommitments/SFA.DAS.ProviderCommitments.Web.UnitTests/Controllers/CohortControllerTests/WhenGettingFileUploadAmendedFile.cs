using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadAmendedFile
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadAmendedFileFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_FileUploadAmendedFileViewModel()
        {
            var fixture = new WhenGettingFileUploadAmendedFileFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadAmendedFileViewModel>();

            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingFileUploadAmendedFileFixture
    {
        private readonly CohortController _sut;
        private readonly FileUploadAmendedFileRequest _request;
        private const long ProviderId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();

        public WhenGettingFileUploadAmendedFileFixture()
        {
            var fixture = new Fixture();
            
            var viewModel = fixture.Create<FileUploadAmendedFileViewModel>();
            _request = new FileUploadAmendedFileRequest { ProviderId = ProviderId, CacheRequestId = _cacheRequestId };

            var modelMapper = new Mock<IModelMapper>();
            modelMapper.Setup(x => x.Map<FileUploadAmendedFileViewModel>(_request)).ReturnsAsync(viewModel);
            _sut = new CohortController(Mock.Of<IMediator>(), modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadAmendedFile(_request);
    }
}