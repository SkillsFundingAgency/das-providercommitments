using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.LearnerControllerTests;

[TestFixture]
public class WhenGettingSelectLearner
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new GetSelectLearnerFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsView()
    {
        var fixture = new GetSelectLearnerFixture();

        var result = await fixture.Act();

        result.VerifyReturnsViewModel().WithModel<SelectLearnerRecordViewModel>();
    }
}

public class GetSelectLearnerFixture
{
    private readonly LearnerController _sut;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly SelectLearnerRecordRequest _request;
    private readonly long _providerId;

    public GetSelectLearnerFixture()
    {
        var fixture = new Fixture();
        _request = new SelectLearnerRecordRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
        _modelMapperMock = new Mock<IModelMapper>();
        var viewModel = fixture.Create<SelectLearnerRecordViewModel>();
        _providerId = 123;

        _modelMapperMock
            .Setup(x => x.Map<SelectLearnerRecordViewModel>(_request))
            .ReturnsAsync(viewModel);

        _sut = new LearnerController(_modelMapperMock.Object);
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<SelectLearnerRecordViewModel>(_request));
    }

    public async Task<IActionResult> Act() => await _sut.SelectLearnerRecord(_request);
}