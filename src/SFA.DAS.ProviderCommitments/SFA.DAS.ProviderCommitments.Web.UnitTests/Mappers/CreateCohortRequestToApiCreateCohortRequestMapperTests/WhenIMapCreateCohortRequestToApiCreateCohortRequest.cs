using SFA.DAS.ProviderCommitments.Web.Mappers;
using WebApp = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestToApiCreateCohortRequestMapperTests;

[TestFixture]
public class WhenIMapCreateCohortRequestToApiCreateCohortRequest
{
    private CreateCohortRequestToApiCreateCohortRequestMapper _mapper;
    private WebApp.CreateCohortRequest _source;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _mapper = new CreateCohortRequestToApiCreateCohortRequestMapper();

        _source = fixture.Build<WebApp.CreateCohortRequest>()
            .Create();
    }

    [Test]
    public async Task ThenAccountIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.AccountId.Should().Be(_source.AccountId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.AccountLegalEntityId.Should().Be(_source.AccountLegalEntityId);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenFirstNameIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.FirstName.Should().Be(_source.FirstName);
    }

    [Test]
    public async Task ThenLastNameIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.LastName.Should().Be(_source.LastName);
    }

    [Test]
    public async Task ThenEmailIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.Email.Should().Be(_source.Email);
    }

    [Test]
    public async Task ThenDateOfBirthIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.DateOfBirth.Should().Be(_source.DateOfBirth);
    }

    [Test]
    public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.Uln.Should().Be(_source.UniqueLearnerNumber);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenCostIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.Cost.Should().Be(_source.Cost);
    }

    [Test]
    public async Task ThenTrainingPriceIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.TrainingPrice.Should().Be(_source.TrainingPrice);
    }

    [Test]
    public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.EndPointAssessmentPrice.Should().Be(_source.EndPointAssessmentPrice);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.StartDate.Should().Be(_source.StartDate);
    }

    [Test]
    public async Task ThenActualStartDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.ActualStartDate.Should().Be(_source.ActualStartDate);
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.EndDate.Should().Be(_source.EndDate);
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.OriginatorReference.Should().Be(_source.OriginatorReference);
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task ThenDeliveryModelIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.IsOnFlexiPaymentPilot.Should().Be(_source.IsOnFlexiPaymentPilot);
    }

    [Test]
    public async Task ThenLearnerDataIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.LearnerDataId.Should().Be(_source.LearnerDataId);
    }
}