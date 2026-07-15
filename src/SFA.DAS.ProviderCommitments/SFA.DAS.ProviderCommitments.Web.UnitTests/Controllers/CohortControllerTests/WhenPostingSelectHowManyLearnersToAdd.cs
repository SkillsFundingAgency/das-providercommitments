using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenPostingSelectHowManyLearnersToAdd
{
    [Test]
    public void Then_RedirectTo_SingleSelectJourney_When_Selected_Option_Is_Single()
    {
        var fixture = new WhenPostingSelectHowManyLearnersToAddFixture();

        var result = fixture.Single().Act();

        result.VerifyReturnsRedirectToActionResult()
            .WithActionName(nameof(CohortController.BeforeYouContinue));
    }

    [Test]
    public void Then_RedirectTo_MultipleSelectJourney_When_Selected_Option_Is_Multiple()
    {
        var fixture = new WhenPostingSelectHowManyLearnersToAddFixture();

        var result = fixture.Multiple().Act();

        result.VerifyReturnsRedirectToActionResult()
            .WithActionName(nameof(CohortController.BeforeYouContinueMultiSelect));
    }
}

public class WhenPostingSelectHowManyLearnersToAddFixture
{
    private readonly CohortController _sut;
    private readonly SelectHowManyLearnersToAddViewModel _viewModel;
    private const long ProviderId = 123;

    public WhenPostingSelectHowManyLearnersToAddFixture()
    {
        _viewModel = new SelectHowManyLearnersToAddViewModel { ProviderId = ProviderId };

        _sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public WhenPostingSelectHowManyLearnersToAddFixture Single()
    {
        _viewModel.Selection = HowManyLearnersToAddOptions.Single;
        return this;
    }

    public WhenPostingSelectHowManyLearnersToAddFixture Multiple()
    {
        _viewModel.Selection = HowManyLearnersToAddOptions.Multiple;
        return this;
    }

    public IActionResult Act() => _sut.SelectHowManyLearnersToAdd(_viewModel);
}