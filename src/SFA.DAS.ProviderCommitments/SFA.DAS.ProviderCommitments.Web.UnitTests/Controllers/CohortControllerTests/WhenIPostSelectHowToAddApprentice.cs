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
    public class WhenIPostSelectHowToAddApprentice
    {
        [Test]
        public void PostHowTo_WithValidModel_WithIlrOption_ShouldRedirectToSelectLearnerRecord()
        {
            var fixture = new WhenIPostSelectHowToAddApprenticeFixture();
            fixture.ViewModel.Selection = AddFirstDraftApprenticeshipJourneyOptions.Ilr;

            var result = fixture.Sut.SelectHowToAddApprentice(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectLearnerRecord");
            var action = result as RedirectToActionResult;
            action.RouteValues["providerId"].Should().Be(fixture.ViewModel.ProviderId);
            action.RouteValues["EmployerAccountLegalEntityPublicHashedId"].Should().Be(fixture.ViewModel.EmployerAccountLegalEntityPublicHashedId);
            action.RouteValues["CacheKey"].Should().Be(fixture.ViewModel.CacheKey);
        }

        [Test]
        public void PostHowTo_WithValidModel_WithManualOption_ShouldRedirectToSelectCourse()
        {
            var fixture = new WhenIPostSelectHowToAddApprenticeFixture();
            fixture.ViewModel.Selection = AddFirstDraftApprenticeshipJourneyOptions.Manual;

            var result = fixture.Sut.SelectHowToAddApprentice(fixture.ViewModel);
            result.VerifyReturnsRedirectToActionResult().WithActionName("SelectCourse");
            var action = result as RedirectToActionResult;
            action.RouteValues["providerId"].Should().Be(fixture.ViewModel.ProviderId);
            action.RouteValues["EmployerAccountLegalEntityPublicHashedId"].Should().Be(fixture.ViewModel.EmployerAccountLegalEntityPublicHashedId);
            action.RouteValues["CacheKey"].Should().Be(fixture.ViewModel.CacheKey);
        }
    }

    public class WhenIPostSelectHowToAddApprenticeFixture
    {
        public readonly SelectHowToAddFirstApprenticeshipJourneyViewModel ViewModel;

        public readonly CohortController Sut;

        public WhenIPostSelectHowToAddApprenticeFixture()
        {
            ViewModel = new SelectHowToAddFirstApprenticeshipJourneyViewModel { ProviderId = 123, EmployerAccountLegalEntityPublicHashedId = "XYZ", CacheKey = Guid.NewGuid()};

            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }
    }
}
