using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenPostingBeforeYouContinueMethod
{

    [Test]
    public void Then_RedirectTo_SelectAddDraftApprenticeshipJourney_When_Continue_Is_Clicked()
    {
        var fixture = new WhenPostingBeforeYouContinueMethodFixture();

        var result = fixture.Act();

        result.VerifyReturnsRedirectToActionResult()
            .WithRouteValue("UseLearnerData", true)
            .WithActionName(nameof(CohortController.SelectAddDraftApprenticeshipJourney));
    }
}

public class WhenPostingBeforeYouContinueMethodFixture
{
    private readonly CohortController _sut;
    private readonly BeforeYouContinueViewModel _viewModel;
    private const long ProviderId = 123;

    public WhenPostingBeforeYouContinueMethodFixture()
    {
        _viewModel = new BeforeYouContinueViewModel { ProviderId = ProviderId };

        _sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public IActionResult Act() => _sut.BeforeYouContinue(_viewModel);
}