using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.AddDraftApprenticeshipToCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapDraftApprenticeshipRequest
    {
        private Web.Mappers.AddDraftApprenticeshipRequestMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Func<Task<AddDraftApprenticeshipApimRequest>> _act;
        private long _cohortId;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            var birthDate = fixture.Create<DateTime?>();
            var startDate = fixture.Create<DateTime?>();
            var actualStartDate = fixture.Create<DateTime?>();
            var employmentEndDate = fixture.Create<DateTime?>();
            var endDate = fixture.Create<DateTime?>();
            var deliveryModel = fixture.Create<DeliveryModel?>();
            var employmentPrice = fixture.Create<int?>();
            _cohortId = fixture.Create<long>();

            _mapper = new AddDraftApprenticeshipRequestMapper();

            _source = fixture.Build<AddDraftApprenticeshipViewModel>()
                .With(x=>x.CohortId, _cohortId)
                .With(x => x.BirthDay, birthDate?.Day)
                .With(x => x.BirthMonth, birthDate?.Month)
                .With(x => x.BirthYear, birthDate?.Year)
                .With(x => x.EmploymentEndMonth, employmentEndDate?.Month)
                .With(x => x.EmploymentEndYear, employmentEndDate?.Year)
                .With(x => x.EndMonth, endDate?.Month)
                .With(x => x.EndYear, endDate?.Year)
                .With(x => x.StartMonth, startDate?.Month)
                .With(x => x.StartYear, startDate?.Year)
                .With(x => x.ActualStartDay, actualStartDate?.Day)
                .With(x => x.ActualStartMonth, actualStartDate?.Month)
                .With(x => x.ActualStartYear, actualStartDate?.Year)
                .With(x => x.DeliveryModel, deliveryModel)
                .With(x => x.EmploymentPrice, employmentPrice)
                .With(x => x.IsOnFlexiPaymentPilot, true)
                .Without(x => x.StartDate)
                .Without(x => x.Courses)
                .Create();

            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task  ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenEmailsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Email, result.Email);
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DateOfBirth.Date, result.DateOfBirth);
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Uln, result.Uln);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            _source.IsOnFlexiPaymentPilot = false;
            var result = await _act();
            Assert.AreEqual(_source.Cost, result.Cost);
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice, result.TrainingPrice);
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndPointAssessmentPrice, result.EndPointAssessmentPrice);
        }

        [Test]
        public async Task ThenCalculatedCostIsMappedCorrectlyForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice + _source.EndPointAssessmentPrice, result.Cost);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartDate.Date, result.StartDate);
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ActualStartDate.Date, result.ActualStartDate);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentEndDate.Date, result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndDate.Date, result.EndDate);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Reference, result.OriginatorReference);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentPrice, result.EmploymentPrice);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
        }
    }
}
