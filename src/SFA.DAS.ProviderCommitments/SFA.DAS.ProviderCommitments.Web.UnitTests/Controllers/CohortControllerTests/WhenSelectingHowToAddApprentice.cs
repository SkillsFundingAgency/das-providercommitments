using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenSelectingHowToAddApprentice
{
    [Test]
    public void ThenReturnsView()
    {
        var fixture = new WhenSelectingHowToAddApprenticeFixture();

        var result = fixture.Act() as ViewResult;

        result.Should().NotBeNull();
        result.Model.GetType().Should().Be(typeof(SelectHowToAddFirstApprenticeshipJourneyViewModel));
        var model = result.Model as SelectHowToAddFirstApprenticeshipJourneyViewModel;
        model.Should().NotBeNull();
        model.ProviderId.Should().Be(fixture.Request.ProviderId);
        model.EmployerAccountLegalEntityPublicHashedId.Should().Be(fixture.Request.EmployerAccountLegalEntityPublicHashedId);
        model.CacheKey.Should().Be(fixture.Request.CacheKey);
    }
}

public class WhenSelectingHowToAddApprenticeFixture
{
    public CohortController Sut { get; set; }
    public readonly CreateCohortWithDraftApprenticeshipRequest Request;

    public WhenSelectingHowToAddApprenticeFixture()
    {
        var fixture = new Fixture();
        Request =  fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();

        Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public IActionResult Act() => Sut.SelectHowToAddApprentice(Request);
}