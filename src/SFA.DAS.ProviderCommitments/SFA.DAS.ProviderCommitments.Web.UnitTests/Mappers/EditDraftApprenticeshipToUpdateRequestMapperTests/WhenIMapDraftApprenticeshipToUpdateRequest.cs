﻿using System;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.EditDraftApprenticeshipToUpdateRequestMapperTests
{
    [TestFixture]
    public class WhenIMapDraftApprenticeshipToUpdateRequest
    {
        private EditDraftApprenticeshipToUpdateRequestMapper _mapper;
        private EditDraftApprenticeshipViewModel _source;
        private Func<Task<UpdateDraftApprenticeshipApimRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            var birthDate = fixture.Create<DateTime?>();
            var startDate = fixture.Create<DateTime?>();
            var endDate = fixture.Create<DateTime?>();

            _mapper = new EditDraftApprenticeshipToUpdateRequestMapper();

            _source = fixture.Build<EditDraftApprenticeshipViewModel>()
                .With(x => x.BirthDay, birthDate?.Day)
                .With(x => x.BirthMonth, birthDate?.Month)
                .With(x => x.BirthYear, birthDate?.Year)
                .With(x => x.EndMonth, endDate?.Month)
                .With(x => x.EndYear, endDate?.Year)
                .With(x => x.StartMonth, startDate?.Month)
                .With(x => x.StartYear, startDate?.Year)
                .With(x => x.IsOnFlexiPaymentPilot, false)
                .Without(x => x.StartDate)
                .Without(x => x.Courses)
                .Create();

            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ReservationId, Is.EqualTo(_source.ReservationId));
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.FirstName, Is.EqualTo(_source.FirstName));
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Email, Is.EqualTo(_source.Email));
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DateOfBirth, Is.EqualTo(_source.DateOfBirth.Date));
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Uln, Is.EqualTo(_source.Uln));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            _source.IsOnFlexiPaymentPilot = false;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.Cost));
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.TrainingPrice, Is.EqualTo(_source.TrainingPrice));
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(_source.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.TrainingPrice + _source.EndPointAssessmentPrice));
        }

        [TestCase(null)]
        [TestCase(1700)]
        public async Task ThenCostIsUnchangedWhenBothTrainingPriceAndEndPointAssessmentPriceAreMissingForPilotProviders(int? cost)
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.TrainingPrice = null;
            _source.EndPointAssessmentPrice = null;
            _source.Cost = cost;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(cost));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProvidersWhenTrainingPriceMissing()
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.TrainingPrice = null;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProvidersWhenEPAMissing()
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.EndPointAssessmentPrice = null;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.TrainingPrice));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.StartDate, Is.EqualTo(_source.StartDate.Date));
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ActualStartDate, Is.EqualTo(_source.ActualStartDate.Date));
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndDate, Is.EqualTo(_source.EndDate.Date));
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Reference, Is.EqualTo(_source.Reference));
        }

        [Test]
        public async Task ThenTheCourseOptionIsMapped()
        {
            var result = await _act();
            Assert.That(result.CourseOption, Is.EqualTo(_source.TrainingCourseOption));
        }

        [Test]
        public async Task ThenIfTheCourseOptionsIsMinusOneThenMappedToEmptyString()
        {
            _source.TrainingCourseOption = "-1";
            var result = await _act();
            Assert.That(result.CourseOption, Is.EqualTo(string.Empty));
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task ThenTheDeliveryModelIsMapped(DeliveryModel dm)
        {
            _source.DeliveryModel = dm;
            var result = await _act();
            Assert.That(result.DeliveryModel, Is.EqualTo(dm));
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentEndDate, Is.EqualTo(_source.EmploymentEndDate.Date));
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentPrice, Is.EqualTo(_source.EmploymentPrice));
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_source.IsOnFlexiPaymentPilot));
        }
    }
}