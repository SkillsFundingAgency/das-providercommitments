using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenGettingBeginMultipleLearnerSelection
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new BeginMultipleLearnerSelectionFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsRedirect()
    {
        var fixture = new BeginMultipleLearnerSelectionFixture();

        var result = await fixture.Act() as RedirectToActionResult;

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("SelectMultipleLearnerRecords");
        redirect.ControllerName.Should().Be("Learner");
    }
}

public class BeginMultipleLearnerSelectionFixture
{
    public CohortController Sut { get; set; }
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly SelectEmployerRedirectRequest _request;
    private readonly long _providerId;
    private readonly Guid _cacheKey;


    public BeginMultipleLearnerSelectionFixture()
    {
        _request = new SelectEmployerRedirectRequest { ProviderId = _providerId };
        _modelMapperMock = new Mock<IModelMapper>();
        var redirectModel = new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = _providerId,
            CacheKey = _cacheKey
        };
        _providerId = 123;
        _cacheKey = Guid.NewGuid();

        _modelMapperMock
            .Setup(x => x.Map<SelectMultipleLearnerRecordsRequest>(_request))
            .ReturnsAsync(redirectModel);

        Sut = new CohortController(Mock.Of<IMediator>(), _modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(), Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<SelectMultipleLearnerRecordsRequest>(_request));
    }

    public async Task<IActionResult> Act() => await Sut.BeginMultipleLearnerSelection(_request);
}