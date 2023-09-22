﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
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
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
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
        public async Task ThenCostIsMappedCorrectlyForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice + _source.EndPointAssessmentPrice, result.Cost);
        }

        [Test]
        public async Task ThenCostIsNullWhenBothTrainingPriceAndEndPointAssessmentPriceAreMissingForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.TrainingPrice = null;
            _source.EndPointAssessmentPrice = null;
            var result = await _act();
            Assert.AreEqual(null, result.Cost);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProvidersWhenTrainingPriceMissing()
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.TrainingPrice = null;
            var result = await _act();
            Assert.AreEqual(_source.EndPointAssessmentPrice, result.Cost);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProvidersWhenEPAMissing()
        {
            _source.IsOnFlexiPaymentPilot = true;
            _source.EndPointAssessmentPrice = null;
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice, result.Cost);
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
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndDate.Date, result.EndDate);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Reference, result.Reference);
        }

        [Test]
        public async Task ThenTheCourseOptionIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_source.TrainingCourseOption, result.CourseOption);
        }

        [Test]
        public async Task ThenIfTheCourseOptionsIsMinusOneThenMappedToEmptyString()
        {
            _source.TrainingCourseOption = "-1";
            var result = await _act();
            Assert.AreEqual(string.Empty, result.CourseOption);
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task ThenTheDeliveryModelIsMapped(DeliveryModel dm)
        {
            _source.DeliveryModel = dm;
            var result = await _act();
            Assert.AreEqual(dm, result.DeliveryModel);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentEndDate.Date, result.EmploymentEndDate);
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