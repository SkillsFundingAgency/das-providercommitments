using System;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequest
    {
        private CreateCohortRequestMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private long _accountLegalEntityId;
        private Func<CreateCohortRequest> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _accountLegalEntityId = fixture.Create<long>();

            var birthDate = fixture.Create<DateTime?>();
            var startDate = fixture.Create<DateTime?>();
            var endDate = fixture.Create<DateTime?>();

            _mapper = new CreateCohortRequestMapper();
            var accountLegalEntity = fixture.Build<AccountLegalEntity>()
                .With(x => x.AccountLegalEntityId, _accountLegalEntityId)
                .Create();

            _source = fixture.Build<AddDraftApprenticeshipViewModel>()
                .With(x => x.AccountLegalEntity, accountLegalEntity)
                .With(x => x.BirthDay, birthDate?.Day)
                .With(x => x.BirthMonth, birthDate?.Month)
                .With(x => x.BirthYear, birthDate?.Year)
                .With(x => x.EndMonth, endDate?.Month)
                .With(x => x.EndYear, endDate?.Year)
                .With(x => x.StartMonth, startDate?.Month)
                .With(x => x.StartYear, startDate?.Year)
                .Without(x => x.StartDate)
                .Without(x => x.Courses)
                .Create();

            _act = () => _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public void ThenReservationIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public void ThenFirstNameIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.FirstName, result.FirstName);
        }

        [Test]
        public void ThenDateOfBirthIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.DateOfBirth.Date, result.DateOfBirth);
        }

        [Test]
        public void ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.Uln, result.UniqueLearnerNumber);
        }

        [Test]
        public void ThenCourseCodeIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public void ThenCostIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.Cost, result.Cost);
        }

        [Test]
        public void ThenStartDateIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.StartDate.Date, result.StartDate);
        }

        [Test]
        public void ThenEndDateIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.EndDate.Date, result.EndDate);
        }

        [Test]
        public void ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.Reference, result.OriginatorReference);
        }

        [Test]
        public void ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_accountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public void ThenProviderIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }
    }
}
