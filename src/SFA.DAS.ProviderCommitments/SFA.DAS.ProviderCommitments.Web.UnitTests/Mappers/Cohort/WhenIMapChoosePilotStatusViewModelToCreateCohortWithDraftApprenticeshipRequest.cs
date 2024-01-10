using System;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
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
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmployerAccountLegalEntityPublicHashedId, Is.EqualTo(_source.EmployerAccountLegalEntityPublicHashedId));
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMapped()
        {
            var result = await _act();
            Assert.That(result.AccountLegalEntityId, Is.EqualTo(_source.AccountLegalEntityId));
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DeliveryModel, Is.EqualTo(_source.DeliveryModel));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ReservationId, Is.EqualTo(_source.ReservationId));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.StartMonthYear, Is.EqualTo(_source.StartMonthYear));
        }

        [TestCase(ChoosePilotStatusOptions.Pilot, true)]
        [TestCase(ChoosePilotStatusOptions.NonPilot, false)]
        public async Task ThenPilotChoiceIsMappedCorrectly(ChoosePilotStatusOptions option, bool expected)
        {
            _source.Selection = option;
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(expected));
        }
    }
}