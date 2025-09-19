using System;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.AddDraftApprenticeshipToCohortRequestMapperTests;

[TestFixture]
public class WhenIMapDraftApprenticeshipRequest
{
    private AddDraftApprenticeshipRequestMapper _mapper;
    private AddDraftApprenticeshipViewModel _source;
    private Func<Task<AddDraftApprenticeshipApimRequest>> _act;
    private long _cohortId;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        var birthDate = fixture.Create<DateTime?>();
        var startDate = fixture.Create<DateTime?>();
        var actualStartDate = fixture.Create<DateTime?>();
        var employmentEndDate = fixture.Create<DateTime?>();
        var endDate = fixture.Create<DateTime?>();
        var deliveryModel = fixture.Create<DeliveryModel?>();
        var employmentPrice = fixture.Create<int?>();
        _cohortId = fixture.Create<long>();

        _mapper = new AddDraftApprenticeshipRequestMapper();

        _source = fixture.Build<AddDraftApprenticeshipViewModel>()
            .With(x=>x.CohortId, _cohortId)
            .With(x => x.BirthDay, birthDate?.Day)
            .With(x => x.BirthMonth, birthDate?.Month)
            .With(x => x.BirthYear, birthDate?.Year)
            .With(x => x.EmploymentEndMonth, employmentEndDate?.Month)
            .With(x => x.EmploymentEndYear, employmentEndDate?.Year)
            .With(x => x.EndMonth, endDate?.Month)
            .With(x => x.EndYear, endDate?.Year)
            .With(x => x.StartMonth, startDate?.Month)
            .With(x => x.StartYear, startDate?.Year)
            .With(x => x.ActualStartDay, actualStartDate?.Day)
            .With(x => x.ActualStartMonth, actualStartDate?.Month)
            .With(x => x.ActualStartYear, actualStartDate?.Year)
            .With(x => x.DeliveryModel, deliveryModel)
            .With(x => x.EmploymentPrice, employmentPrice)
            .Without(x => x.StartDate)
            .Without(x => x.Courses)
            .Create();

        _act = async () => await _mapper.Map(TestHelper.Clone(_source));
    }

    [Test]
    public async Task  ThenReservationIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task ThenFirstNameIsMappedCorrectly()
    {
        var result = await _act();
        result.FirstName.Should().Be(_source.FirstName);
    }

    [Test]
    public async Task ThenEmailsMappedCorrectly()
    {
        var result = await _act();
        result.Email.Should().Be(_source.Email);
    }

    [Test]
    public async Task ThenDateOfBirthIsMappedCorrectly()
    {
        var result = await _act();
        result.DateOfBirth.Should().Be(_source.DateOfBirth.Date);
    }

    [Test]
    public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
    {
        var result = await _act();
        result.Uln.Should().Be(_source.Uln);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenCostIsMappedCorrectly()
    {
        var result = await _act();
        result.Cost.Should().Be(_source.Cost);
    }

    [Test]
    public async Task ThenTrainingPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.TrainingPrice.Should().Be(_source.TrainingPrice);
    }

    [Test]
    public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EndPointAssessmentPrice.Should().Be(_source.EndPointAssessmentPrice);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartDate.Should().Be(_source.StartDate.Date);
    }

    [Test]
    public async Task ThenActualStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.ActualStartDate.Should().Be(_source.ActualStartDate.Date);
    }

    [Test]
    public async Task ThenEmploymentEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentEndDate.Should().Be(_source.EmploymentEndDate.Date);
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EndDate.Should().Be(_source.EndDate.Date);
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.OriginatorReference.Should().Be(_source.Reference);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenDeliveryModelIsMappedCorrectly()
    {
        var result = await _act();
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ThenEmploymentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentPrice.Should().Be(_source.EmploymentPrice);
    }
}