using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;

[TestFixture]
public class WhenIMapAddDraftApprenticeshipViewModelToReservationsAddDraftApprenticeshipRequest
{
    private ReservationsAddDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModelMapper _mapper;
    private AddDraftApprenticeshipViewModel _source;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Build<AddDraftApprenticeshipViewModel>()
            .Without(x => x.BirthDay)
            .Without(x => x.BirthMonth)
            .Without(x => x.BirthYear)
            .Without(x => x.StartMonth)
            .Without(x => x.StartYear)
            .Without(x => x.EndMonth)
            .Without(x => x.EndYear)
            .With(x => x.StartDate, new MonthYearModel("012019"))
            .With(x => x.CohortId, fixture.Create<long>())
            .Create();

        _mapper = new ReservationsAddDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModelMapper();
    }

    [Test]
    public async Task ProviderIdIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task CourseCodeIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task DeliveryModelIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ReservationIdIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task StartMonthYearIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.StartMonthYear.Should().Be(_source.StartDate.MonthYear);
    }

    [Test]
    public async Task CohortIdIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.CohortId.Should().Be(_source.CohortId);
    }

    [Test]
    public async Task CohortReferenceIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.CohortReference.Should().Be(_source.CohortReference);
    }

    [Test]
    public async Task EmployerAccountLegalEntityPublicHashedIdIsMapped()
    {
        var result = await _mapper.Map(_source);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public async Task UseLearnerDataIsFalse()
    {
        var result = await _mapper.Map(_source);
        result.UseLearnerData.Should().BeFalse();
    }
}
