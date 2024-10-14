using System;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapChoosePilotStatusViewModelToCreateCohortWithDraftApprenticeshipRequest
{
    private CreateCohortWithDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper _mapper;
    private ChoosePilotStatusViewModel _source;
    private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Create<ChoosePilotStatusViewModel>();

        _mapper = new CreateCohortWithDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper();

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMapped()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(_source.AccountLegalEntityId);
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
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartMonthYear.Should().Be(_source.StartMonthYear);
    }

    [TestCase(ChoosePilotStatusOptions.Pilot, true)]
    [TestCase(ChoosePilotStatusOptions.NonPilot, false)]
    public async Task ThenPilotChoiceIsMappedCorrectly(ChoosePilotStatusOptions option, bool expected)
    {
        _source.Selection = option;
        var result = await _act();
        result.IsOnFlexiPaymentPilot.Should().Be(expected);
    }
}