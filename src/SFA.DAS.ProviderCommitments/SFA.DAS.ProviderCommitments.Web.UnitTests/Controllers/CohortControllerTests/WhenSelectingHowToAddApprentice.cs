using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenSelectingHowToAddApprentice
{
    [Test]
    public void ThenReturnsRedirectToSelectFromIlr()
    {
        var fixture = new WhenSelectingHowToAddApprenticeFixture();

        var result = fixture.Act() as RedirectToActionResult;

        result.Should().NotBeNull();

        result.RouteValues["ProviderId"].Should().Be(fixture.Request.ProviderId);
        result.RouteValues["EmployerAccountLegalEntityPublicHashedId"].Should().Be(fixture.Request.EmployerAccountLegalEntityPublicHashedId);
        result.RouteValues["CacheKey"].Should().Be(fixture.Request.CacheKey);
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