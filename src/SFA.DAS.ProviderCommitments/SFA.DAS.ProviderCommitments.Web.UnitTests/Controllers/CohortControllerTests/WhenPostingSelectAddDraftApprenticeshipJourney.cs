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
    public class WhenPostingSelectAddDraftApprenticeshipJourney
    {
        [Test]
        public void Then_RedirectTo_SelectEmployer_When_Selected_Option_Is_New_Cohort()
        {
            var fixture = new WhenPostingSelectAddDraftApprenticeshipJourneyFixture();

            var result = fixture.CreateNewCohort().Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectEmployer");
            var action = result as RedirectToActionResult;
            action.RouteValues["providerId"].Should().Be(fixture.ViewModel.ProviderId);
        }

        [Test]
        public void Then_RedirectTo_ChooseCohort_When_Selected_Option_Is_ExistingCohort()
        {
            var fixture = new WhenPostingSelectAddDraftApprenticeshipJourneyFixture();

            var result = fixture.AddToExistingCohort().Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("ChooseCohort");
            var action = result as RedirectToActionResult;
            action.RouteValues["providerId"].Should().Be(fixture.ViewModel.ProviderId);
        }
    }

    public class WhenPostingSelectAddDraftApprenticeshipJourneyFixture
    {
        private readonly CohortController _sut;
        public readonly SelectAddDraftApprenticeshipJourneyViewModel ViewModel;
        private const long ProviderId = 123;

        public WhenPostingSelectAddDraftApprenticeshipJourneyFixture()
        {
            var fixture = new Fixture();
            ViewModel = fixture.Create< SelectAddDraftApprenticeshipJourneyViewModel>();

            _sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }

        public WhenPostingSelectAddDraftApprenticeshipJourneyFixture CreateNewCohort()
        {
            ViewModel.Selection = AddDraftApprenticeshipJourneyOptions.NewCohort;
            return this;
        }

        public WhenPostingSelectAddDraftApprenticeshipJourneyFixture AddToExistingCohort()
        {
            ViewModel.Selection = AddDraftApprenticeshipJourneyOptions.ExistingCohort;
            return this;
        }

        public IActionResult Act() => _sut.SelectAddDraftApprenticeshipJourney(ViewModel);
    }
}