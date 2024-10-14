using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapEditDraftApprenticeshipViewModelToValidateDraftApprenticeshipApimRequestTest
{
    private EditDraftApprenticeshipViewModelToValidateApimRequestMapper _mapper;
    private EditDraftApprenticeshipViewModel _source;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _mapper = new EditDraftApprenticeshipViewModelToValidateApimRequestMapper();

        _source = fixture.Build<EditDraftApprenticeshipViewModel>()
                
            .With(x => x.BirthDay, 1).With(x => x.BirthMonth, 1).With(x => x.BirthYear, 2000)
            .Without(x => x.StartDate)
            .With(x => x.StartMonth,2).With(x => x.StartYear, 2020)
            .With(x => x.IsOnFlexiPaymentPilot, false)
            .With(x => x.EndMonth, 1)
            .With(x => x.EndYear, 2022)
            .Create();
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
        result.DateOfBirth.Should().Be(_source.DateOfBirth.Date);
    }

    [Test]
    public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.Uln.Should().Be(_source.Uln);
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
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.StartDate.Should().Be(_source.StartDate.Date);
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.EndDate.Should().Be(_source.EndDate.Date);
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.OriginatorReference.Should().Be(_source.Reference);
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
    public async Task ThenActualStartDateIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.ActualStartDate.Should().Be(_source.ActualStartDate.Date);
    }
}