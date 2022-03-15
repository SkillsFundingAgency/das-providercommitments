using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers

{
    [TestFixture]
    public class WhenIMapAddDraftApprenticeshipViewModelToBaseReservationsAddDraftApprenticeshipRequest
    {
        private BaseReservationsAddDraftApprenticeshipRequestMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Func<Task<BaseReservationsAddDraftApprenticeshipRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<AddDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x=>x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            _source.StartDate = new MonthYearModel("092022");

            _mapper = new BaseReservationsAddDraftApprenticeshipRequestMapper();

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
            Assert.AreEqual(_source.StartDate.MonthYear, result.StartMonthYear);
        }
    }
}
