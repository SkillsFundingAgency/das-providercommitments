using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapSelectCourseViewModelToCreateCohortWithDraftApprenticeshipRequest
    {
        private CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper _mapper;
        private SelectCourseViewModel _source;
        private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<SelectCourseViewModel>();

            _mapper = new CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper();

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_source.AccountLegalEntityId, result.AccountLegalEntityId);
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
