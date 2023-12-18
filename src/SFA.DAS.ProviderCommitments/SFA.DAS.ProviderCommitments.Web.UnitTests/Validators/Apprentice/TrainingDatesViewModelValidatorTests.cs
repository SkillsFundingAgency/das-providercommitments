using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class TrainingDatesViewModelValidatorTests
    {
        private Mock<IAcademicYearDateProvider> _mockAcademicYearDateProvider;

        [SetUp]
        public void Arrange()
        {
            _mockAcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();

            _mockAcademicYearDateProvider.Setup(p => p.CurrentAcademicYearEndDate).Returns(new DateTime(2020, 7, 31));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new TrainingDatesViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new TrainingDatesViewModel { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [Test]
        public void AndStopDateIsValid_ThenShouldNotHaveError()
        {
            DateTime? date = new DateTime(2020, 1, 1);
            var model = new TrainingDatesViewModel { StopDate = date };
            AssertValidationResult(request => request.StopDate, model, true);
        }

        [Test]
        public void AndStartDateIsValid_ThenShouldNotHaveError()
        {
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2020;
            var model = new TrainingDatesViewModel { StartDate = startDate, StopDate = null };
            AssertValidationResult(request => request.StartDate, model, true);
        }

        [Test]
        public void AndStartDateIsEmpty_ThenShouldHaveError()
        {
            MonthYearModel startDate = new MonthYearModel("");
            var model = new TrainingDatesViewModel { StartDate = startDate, StopDate = null };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase(2020, 15)]
        [TestCase(2020, null)]
        [TestCase(0, 12)]
        [TestCase(null, 12)]
        public void AndStartDateIsInvalid_ThenShouldHaveError(int? year, int? month)
        {
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = month;
            startDate.Year = year;
            var model = new TrainingDatesViewModel { StartDate = startDate, StopDate = null };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [Test]
        public void AndStartDateIsBeforeStopDate_ThenShouldHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2018;
            var model = new TrainingDatesViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase("082020", null, true)]
        [TestCase("082020", "082020", false)]
        [TestCase("082020", "072020", false)]
        [TestCase("082020", "072021", true)]
        public void AndStartDateIsComparedAgainstEndDate_ThenShouldGetExpectedResult(string startDate, string endDate,
            bool expected)
        {
            MonthYearModel start = new MonthYearModel(startDate);
            MonthYearModel end = new MonthYearModel(endDate);
            var model = new TrainingDatesViewModel { StartDate = start, StopDate = null, EndDate = end };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        [TestCase("082020", true)]
        [TestCase("072021", true)]
        [TestCase("082021", false)]
        public void AndStartDateIsComparedAgainstAcademicYear_ThenShouldGetExpectedResult(string startDate,
            bool expected)
        {
            MonthYearModel start = new MonthYearModel(startDate);
            var model = new TrainingDatesViewModel { StartDate = start, StopDate = null };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        private void AssertValidationResult<T>(Expression<Func<TrainingDatesViewModel, T>> property,
            TrainingDatesViewModel instance, bool expectedValid)
        {
            var validator = new TrainingDatesViewModelValidator(_mockAcademicYearDateProvider.Object);

            if (expectedValid)
            {
                validator.ShouldNotHaveValidationErrorFor(property, instance);
            }
            else
            {
                validator.ShouldHaveValidationErrorFor(property, instance);
            }
        }


        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("32020", true)]
        [TestCase("032020", true)]
        public void AndStartDateIsValid_ThenShouldNotHaveError(string startDate, bool expected)
        {
            var model = new TrainingDatesViewModel { StartDate = new MonthYearModel(startDate) };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        [Test]
        public void AndEndDateIsEmpty_ThenShouldHaveError()
        {
            MonthYearModel endDate = new MonthYearModel("");
            var model = new TrainingDatesViewModel { EndDate = endDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase(2020, 15)]
        [TestCase(2020, null)]
        [TestCase(0, 12)]
        [TestCase(null, 12)]
        public void AndEndDateIsInvalid_ThenShouldHaveError(int? year, int? month)
        {
            MonthYearModel endDate = new MonthYearModel("");
            endDate.Month = month;
            endDate.Year = year;
            var model = new TrainingDatesViewModel { EndDate = endDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase("012020", "022020", true)]
        [TestCase("012020", "012020", false)]
        [TestCase("012020", "012019", false)]
        public void AndWhenEndDateIsCheckedAgainstStartDate_ThenShouldHaveExpectedResult(string startDate,
            string endDate, bool expected)
        {
            var model = new TrainingDatesViewModel
            { StartDate = new MonthYearModel(startDate), EndDate = new MonthYearModel(endDate) };
            AssertValidationResult(request => request.EndDate, model, expected);
        }

        [TestCase("042020", "052020", null)]
        [TestCase("042020", "032020",
            "This date must not be later than the projected apprenticeship training end date")]
        [TestCase("012019", "122020", "This date must be at least 3 months later than the employment start date")]
        [TestCase("032020", "122020", "This date must be at least 3 months later than the employment start date")]
        public void AndEmploymentEndDateIsNotLaterThanTrainingEndDate(string employmentEndDate, string trainingEndDate,
            string error)
        {
            var model = new TrainingDatesViewModel
            {
                DeliveryModel = DeliveryModel.PortableFlexiJob,
                StartDate = new MonthYearModel("012020"),
                EndDate = new MonthYearModel(trainingEndDate),
                EmploymentEndDate = new MonthYearModel(employmentEndDate),
            };

            var result = new TrainingDatesViewModelValidator(_mockAcademicYearDateProvider.Object).TestValidate(model);

            if (error == null)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.EmploymentEndDate);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.EmploymentEndDate).WithErrorMessage(error);
            }
        }

        [Test]
        public void AndEmploymentEndDateIsNotLaterThanTrainingEndDateWhenBeforeStartDate()
        {
            var model = new TrainingDatesViewModel
            {
                DeliveryModel = DeliveryModel.PortableFlexiJob,
                StartDate = new MonthYearModel("042022"),
                EmploymentEndDate = new MonthYearModel("022023"),
                EndDate = new MonthYearModel("022022"),
            };

            var result = new TrainingDatesViewModelValidator(_mockAcademicYearDateProvider.Object).TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                .WithErrorMessage("This date must not be before the end date for this employment")
                .WithoutErrorMessage("This date must be later than the employment start date");
        }
    }
}