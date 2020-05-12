using FluentValidation.TestHelper;
using NUnit.Framework;
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
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new StartDateViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void ThenEmployerAccountLegalEntityPublicHashedIdIsValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new StartDateViewModel { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };

            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(102, true)]
        public void ThenAccountLegalEntityIdIsValidated(long accountLegalEntityId, bool expectedValid)
        {
            var model = new StartDateViewModel { AccountLegalEntityId = accountLegalEntityId };
            AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
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
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel start = new MonthYearModel(startDate);
            var model = new StartDateViewModel { StartDate = start, StopDate = stopDate, EndDate = endDate };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        private void AssertValidationResult<T>(Expression<Func<StartDateViewModel, T>> property, StartDateViewModel instance, bool expectedValid)
        {
            var validator = new StartDateViewModelValidator();

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