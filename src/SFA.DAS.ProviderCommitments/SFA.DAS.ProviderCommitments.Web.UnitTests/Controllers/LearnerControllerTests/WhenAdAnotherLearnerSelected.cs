using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.LearnerControllerTests;

[TestFixture]
public class WhenAdAnotherLearnerSelected
{
    [Test]
    public async Task ThenCallsModelMapper()
    {
        var fixture = new WhenAdAnotherLearnerSelectedFixture();

        await fixture.Act();

        fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsView()
    {
        var fixture = new WhenAdAnotherLearnerSelectedFixture();

        var result = await fixture.Act();

        result.VerifyReturnsRedirectToActionResult().WithActionName("AddDraftApprenticeship").WithControllerName("DraftApprenticeship");
    }
}

public class WhenAdAnotherLearnerSelectedFixture
{
    private readonly LearnerController _sut;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly AddAnotherLearnerSelectedRequest _request;
    private readonly ReservationsAddDraftApprenticeshipRequest _model;
    private readonly long _providerId;
    private readonly long _learnerDataId;
    private readonly string _cohortReference;

    public WhenAdAnotherLearnerSelectedFixture()
    {
        var fixture = new Fixture();
        _providerId = fixture.Create<long>();
        _learnerDataId = fixture.Create<long>();
        _cohortReference = fixture.Create<string>();
        _model = fixture.Create<ReservationsAddDraftApprenticeshipRequest>();
        _request = new AddAnotherLearnerSelectedRequest { ProviderId = _providerId, CohortReference =  _cohortReference, LearnerDataId = _learnerDataId};
        _modelMapperMock = new Mock<IModelMapper>();

        _modelMapperMock
            .Setup(x => x.Map<ReservationsAddDraftApprenticeshipRequest>(_request))
            .ReturnsAsync(_model);

        _sut = new LearnerController(_modelMapperMock.Object);
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<ReservationsAddDraftApprenticeshipRequest>(_request));
    }

    public async Task<IActionResult> Act() => await _sut.LearnerToBeAddedToCohort(_request);
}