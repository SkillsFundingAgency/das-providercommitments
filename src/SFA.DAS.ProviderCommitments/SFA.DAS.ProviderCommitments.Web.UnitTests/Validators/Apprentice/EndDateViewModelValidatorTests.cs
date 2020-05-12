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
    public class EndDateViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new EndDateViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void ThenEmployerAccountLegalEntityPublicHashedIdIsValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new EndDateViewModel { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };

            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(102, true)]
        public void ThenAccountLegalEntityIdIsValidated(long accountLegalEntityId, bool expectedValid)
        {
            var model = new EndDateViewModel { AccountLegalEntityId = accountLegalEntityId };
            AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new EndDateViewModel { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("XXXX", false)]
        [TestCase("32020", true)]
        [TestCase("032020", true)]
        public void AndStartDateIsValid_ThenShouldNotHaveError(string startDate, bool expected)
        {
            var model = new EndDateViewModel { StartDate = startDate };
            AssertValidationResult(request => request.StartDate, model, expected);
        }

        [Test]
        public void AndEndDateIsEmpty_ThenShouldHaveError()
        {
            MonthYearModel endDate = new MonthYearModel("");
            var model = new EndDateViewModel { EndDate = endDate };
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
            var model = new EndDateViewModel { EndDate = endDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase("012020", "022020", true)]
        [TestCase("012020", "012020", false)]
        [TestCase("012020", "012019", false)]
        public void AndWhenEndDateIsCheckedAgainstStartDate_ThenShouldHaveExpectedResult(string startDate, string endDate, bool expected)
        {
            var model = new EndDateViewModel { StartDate = startDate, EndDate = new MonthYearModel(endDate) };
            AssertValidationResult(request => request.EndDate, model, expected);
        }

        private void AssertValidationResult<T>(Expression<Func<EndDateViewModel, T>> property, EndDateViewModel instance, bool expectedValid)
        {
            var validator = new EndDateViewModelValidator();

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