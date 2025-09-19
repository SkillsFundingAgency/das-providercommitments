using System;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;

[TestFixture]
public class WhenIMapSelectCourseViewModelToReservationsAddDraftApprenticeshipRequest
{
    private ReservationsAddDraftApprenticeshipRequestFromSelectCourseViewModelMapper _mapper;
    private SelectCourseViewModel _source;
    private Func<Task<ReservationsAddDraftApprenticeshipRequest>> _act;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Create<SelectCourseViewModel>();
        _source.StartMonthYear = "092022";

        _mapper = new ReservationsAddDraftApprenticeshipRequestFromSelectCourseViewModelMapper();

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenCohortReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.CohortReference.Should().Be(_source.CohortReference);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenDeliveryModelIsMappedCorrectly()
    {
        var result = await _act();
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_source.ReservationId.Value);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartMonthYear.Should().Be(_source.StartMonthYear);
    }
}