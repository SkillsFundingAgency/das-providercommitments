using System;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Tests.Mappers.CreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequest
    {
        private CreateCohortRequestMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Func<CreateCohortRequest> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            var birthDate = fixture.Create<DateTime?>();
            var startDate = fixture.Create<DateTime?>();
            var endDate = fixture.Create<DateTime?>();

            _mapper = new CreateCohortRequestMapper();
            _source = fixture.Build<AddDraftApprenticeshipViewModel>()
                .With(x => x.BirthDay, birthDate?.Day)
                .With(x => x.BirthMonth, birthDate?.Month)
                .With(x => x.BirthYear, birthDate?.Year)
                .With(x => x.FinishMonth, endDate?.Month)
                .With(x => x.FinishYear, endDate?.Year)
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
            Assert.AreEqual(_source.BirthDate.Date, result.DateOfBirth);
        }


        [Test]
        public void ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.UniqueLearnerNumber, result.UniqueLearnerNumber);
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
            Assert.AreEqual(_source.FinishDate.Date, result.EndDate);
        }

        [Test]
        public void ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.Reference, result.OriginatorReference);
        }

        [Test]
        public void ThenEmployerAccountIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.EmployerAccountId, result.EmployerAccountId);
        }
        [Test]
        public void ThenLegalEntityIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.LegalEntityId, result.LegalEntityId);
        }
        [Test]
        public void ThenProviderIdIsMappedCorrectly()
        {
            var result = _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }
    }
}
