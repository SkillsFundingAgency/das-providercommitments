using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.LearnerControllerTests;

[TestFixture]
public class WhenGettingSelectMultipleLearnerRecords
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new GetSelectMultipleLearnerRecordsFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsView()
    {
        var fixture = new GetSelectMultipleLearnerRecordsFixture();

        var result = await fixture.Act();

        result.VerifyReturnsViewModel().WithModel<SelectMultipleLearnerRecordsViewModel>();
    }
}

public class GetSelectMultipleLearnerRecordsFixture
{
    private readonly LearnerController _sut;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly SelectMultipleLearnerRecordsRequest _request;
    private readonly SelectMultipleLearnerRecordsViewModel _viewModel;

    public GetSelectMultipleLearnerRecordsFixture()
    {
        var fixture = new Fixture();
        _modelMapperMock = new Mock<IModelMapper>();

        var providerId = 123;
        _request = new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = providerId,
        };

        _viewModel = fixture.Create<SelectMultipleLearnerRecordsViewModel>();

        _modelMapperMock
            .Setup(x => x.Map<SelectMultipleLearnerRecordsViewModel>(_request))
            .ReturnsAsync(_viewModel);

        _sut = new LearnerController(_modelMapperMock.Object);
    }

    public async Task<IActionResult> Act() => await _sut.SelectMultipleLearnerRecords(_request);

    public void VerifyMapperWasCalled()
        => _modelMapperMock.Verify(x => x.Map<SelectMultipleLearnerRecordsViewModel>(_request), Times.Once);
}