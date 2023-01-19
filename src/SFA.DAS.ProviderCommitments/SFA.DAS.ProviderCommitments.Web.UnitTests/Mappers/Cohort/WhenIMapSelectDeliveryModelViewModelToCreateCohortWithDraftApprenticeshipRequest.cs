using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelToCreateCohortWithDraftApprenticeshipRequest
    {
        private CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper _mapper;
        private SelectDeliveryModelViewModel _source;
        private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<SelectDeliveryModelViewModel>();

            _mapper = new CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper();

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

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.IsOnFlexiPaymentsPilot, result.IsOnFlexiPaymentPilot);
        }
    }
}
