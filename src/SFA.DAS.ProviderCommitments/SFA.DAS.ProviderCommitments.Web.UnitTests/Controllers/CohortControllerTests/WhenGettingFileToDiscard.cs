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
    public class WhenGettingFileToDiscard
    {
        [Test]
        public void ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingFileToDiscardFixture();

            //Act
            var result =  fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenReturnsView_With_FileDiscardViewModel()
        {
            //Arrange
            var fixture = new WhenGettingFileToDiscardFixture();

            //Act
            var viewResult = fixture.Act();

            //Assert
            var model = viewResult.VerifyReturnsViewModel().WithModel<FileDiscardViewModel>();
            Assert.That(model, Is.Not.Null);
        }
    }

    public class WhenGettingFileToDiscardFixture
    {
        private readonly CohortController _sut;
        private readonly FileDiscardRequest _request;
        private const long ProviderId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();

        public WhenGettingFileToDiscardFixture()
        {
            var fixture = new Fixture();

            var viewModel = fixture.Create<FileDiscardViewModel>();
            _request = new FileDiscardRequest { ProviderId = ProviderId, CacheRequestId = _cacheRequestId };

            var modelMapper = new Mock<IModelMapper>();
            modelMapper.Setup(x => x.Map<FileDiscardViewModel>(_request)).ReturnsAsync(viewModel);
            _sut = new CohortController(Mock.Of<IMediator>(), modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public IActionResult Act() => _sut.FileUploadDiscard(_request);
    }
}
