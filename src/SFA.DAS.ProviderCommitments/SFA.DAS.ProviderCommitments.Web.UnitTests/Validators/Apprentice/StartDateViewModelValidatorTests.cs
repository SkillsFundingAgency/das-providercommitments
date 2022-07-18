using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class StartDateViewModelValidatorTests
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
            var request = new StartDateViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new StartDateViewModel { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [Test]
        public void AndStopDateIsValid_ThenShouldNotHaveError()
        {
            DateTime? date = new DateTime(2020, 1, 1);
            var model = new StartDateViewModel { StopDate = date };
            AssertValidationResult(request => request.StopDate, model, true);
        }

        [Test]
        public void AndStopDateIsNull_ThenShouldHaveError()
        {
            var model = new StartDateViewModel { StopDate = null };
            AssertValidationResult(request => request.StopDate, model, false);
        }


        [Test]
        public void AndStartDateIsValid_ThenShouldNotHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2020;
            var model = new StartDateViewModel {StartDate = startDate, StopDate = stopDate};
            AssertValidationResult(request => request.StartDate, model, true);
        }

        [Test]
        public void AndStartDateIsEmpty_ThenShouldHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            var model = new StartDateViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase(2020, 15)]
        [TestCase(2020, null)]
        [TestCase(0, 12)]
        [TestCase(null, 12)]
        public void AndStartDateIsInvalid_ThenShouldHaveError(int? year, int? month)
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = month;
            startDate.Year = year;
            var model = new StartDateViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [Test]
        public void AndStartDateIsBeforeStopDate_ThenShouldHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2018;
            var model = new StartDateViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase("082020", null, true)]
        [TestCase("082020", "082020", false)]
        [TestCase("082020", "072020", false)]
        [TestCase("082020", "072021", true)]
        public void AndStartDateIsComparedAgainstEndDate_ThenShouldGetExpectedResult(string startDate, string endDate, bool expected)
        {
            DateTime? stopDate = new DateTime(2020, 1, 1);
            MonthYearModel start = new MonthYearModel(startDate);
            var model = new StartDateViewModel { StartDate = start, StopDate = stopDate, EndDate = endDate };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        [TestCase("082020", true)]
        [TestCase("072021", true)]
        [TestCase("082021", false)]
        public void AndStartDateIsComparedAgainstAcademicYear_ThenShouldGetExpectedResult(string startDate, bool expected)
        {
            DateTime? stopDate = new DateTime(2020, 1, 1);
            MonthYearModel start = new MonthYearModel(startDate);
            var model = new StartDateViewModel { StartDate = start, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        private void AssertValidationResult<T>(Expression<Func<StartDateViewModel, T>> property, StartDateViewModel instance, bool expectedValid)
        {
            var validator = new StartDateViewModelValidator(_mockAcademicYearDateProvider.Object);

            if (expectedValid)
            {
                validator.ShouldNotHaveValidationErrorFor(property, instance);
            }
            else
            {
                validator.ShouldHaveValidationErrorFor(property, instance);
            }
        }
    }
}