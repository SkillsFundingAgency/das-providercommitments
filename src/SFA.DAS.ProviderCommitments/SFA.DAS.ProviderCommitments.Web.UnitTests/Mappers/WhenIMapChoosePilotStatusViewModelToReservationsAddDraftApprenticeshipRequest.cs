using System;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapChoosePilotStatusViewModelToReservationsAddDraftApprenticeshipRequest
    {
        private ReservationsAddDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper _mapper;
        private ChoosePilotStatusViewModel _source;
        private Func<Task<ReservationsAddDraftApprenticeshipRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<ChoosePilotStatusViewModel>();
            _source.StartMonthYear = "092022";

            _mapper = new ReservationsAddDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper();

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CohortReference, Is.EqualTo(_source.CohortReference));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
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
        [TestCase(null, null)]
        public async Task ThenPilotStatusIsMappedCorrectly(ChoosePilotStatusOptions? option, bool? expected)
        {
            _source.Selection = option;
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentsPilot, Is.EqualTo(expected));
        }
    }
}