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
    public class WhenGettingFileUploadStart
    {
        [Test]
        public void ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

            var result = fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

            var viewResult = fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadStartViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }
    }

    public class WhenGettingFileUploadStartFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;

        public WhenGettingFileUploadStartFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public IActionResult Act() => Sut.FileUploadStart(_request);
    }
}