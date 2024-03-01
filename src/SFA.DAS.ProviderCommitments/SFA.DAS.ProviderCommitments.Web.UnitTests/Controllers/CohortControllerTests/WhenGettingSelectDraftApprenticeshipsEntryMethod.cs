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
    public class WhenGettingSelectDraftApprenticeshipsEntryMethod
    {
        [Test]
        public void ThenReturnsView()
        {
            var fixture = new WhenGettingSelectDraftApprenticeshipsEntryMethodFixture();

            var result = fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var viewResult = await fixture.ActAsync();

            var model = viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.That(model.ProviderId, Is.EqualTo(fixture.ProviderId));
        }
    }

    public class WhenGettingSelectDraftApprenticeshipsEntryMethodFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;

        public WhenGettingSelectDraftApprenticeshipsEntryMethodFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<SFA.DAS.ProviderCommitments.Web.Authorization.IAuthorizationService>());
        }

        public IActionResult Act() => Sut.SelectDraftApprenticeshipsEntryMethod(_request);
    }
}