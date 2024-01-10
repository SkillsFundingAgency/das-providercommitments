using System;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapAddDraftApprenticeshipViewModelToBaseReservationsAddDraftApprenticeshipRequest
    {
        private ReservationsAddDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper _mapper;
        private SelectDeliveryModelViewModel _source;
        private Func<Task<BaseReservationsAddDraftApprenticeshipRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<SelectDeliveryModelViewModel>();
            _source.StartMonthYear = "092022";

            _mapper = new ReservationsAddDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper();

            _act = async () => await _mapper.Map(_source);
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

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentsPilot, Is.EqualTo(_source.IsOnFlexiPaymentsPilot));
        }
    }
}
