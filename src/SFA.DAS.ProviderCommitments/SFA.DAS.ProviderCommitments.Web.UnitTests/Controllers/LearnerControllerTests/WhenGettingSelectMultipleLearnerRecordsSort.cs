using Microsoft.AspNetCore.Routing;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.LearnerControllerTests;

[TestFixture]
public class WhenGettingSelectMultipleLearnerRecordsSort
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new GetSelectMultipleLearnerRecordsSortFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenRedirectsToSelectMultipleLearnerRecords_WithMappedRouteValues()
    {
        var fixture = new GetSelectMultipleLearnerRecordsSortFixture();

        var result = await fixture.Act();

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("SelectMultipleLearnerRecords");
        redirect.ControllerName.Should().BeNull(); // same controller
        redirect.RouteValues.Should().BeEquivalentTo(fixture.ExpectedRouteValues);
    }
}

public class GetSelectMultipleLearnerRecordsSortFixture
{
    private readonly LearnerController _sut;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly SelectMultipleLearnerRecordsSortRequest _request;
    private readonly SelectMultipleLearnerRecordsRequest _redirectRequest;

    public RouteValueDictionary ExpectedRouteValues { get; }

    public GetSelectMultipleLearnerRecordsSortFixture()
    {
        var fixture = new Fixture();

        _modelMapperMock = new Mock<IModelMapper>();

        _request = fixture.Create<SelectMultipleLearnerRecordsSortRequest>();
        _redirectRequest = fixture.Create<SelectMultipleLearnerRecordsRequest>();

        ExpectedRouteValues = new RouteValueDictionary(_redirectRequest);

        _modelMapperMock
            .Setup(x => x.Map<SelectMultipleLearnerRecordsRequest>(_request))
            .ReturnsAsync(_redirectRequest);

        _sut = new LearnerController(_modelMapperMock.Object);
    }

    public Task<IActionResult> Act() => _sut.SelectMultipleLearnerRecordsSort(_request);

    public void VerifyMapperWasCalled()
        => _modelMapperMock.Verify(x => x.Map<SelectMultipleLearnerRecordsRequest>(_request), Times.Once);
}