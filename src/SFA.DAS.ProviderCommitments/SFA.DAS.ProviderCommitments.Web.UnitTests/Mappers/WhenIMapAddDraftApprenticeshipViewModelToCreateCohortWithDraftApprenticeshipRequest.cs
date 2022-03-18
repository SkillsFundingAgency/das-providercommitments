using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
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
    }
}
