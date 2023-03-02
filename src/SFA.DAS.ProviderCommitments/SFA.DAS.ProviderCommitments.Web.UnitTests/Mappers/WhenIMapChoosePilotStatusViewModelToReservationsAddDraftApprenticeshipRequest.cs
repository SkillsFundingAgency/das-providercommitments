using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
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
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartMonthYear, result.StartMonthYear);
        }

        [TestCase(ChoosePilotStatusOptions.Pilot, true)]
        [TestCase(ChoosePilotStatusOptions.NonPilot, false)]
        [TestCase(null, null)]
        public async Task ThenPilotStatusIsMappedCorrectly(ChoosePilotStatusOptions? option, bool? expected)
        {
            _source.Selection = option;
            var result = await _act();
            Assert.AreEqual(expected, result.IsOnFlexiPaymentsPilot);
        }
    }
}