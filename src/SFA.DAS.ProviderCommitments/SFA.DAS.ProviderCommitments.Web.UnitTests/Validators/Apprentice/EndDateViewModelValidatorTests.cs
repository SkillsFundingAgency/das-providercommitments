using FluentValidation.TestHelper;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

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

        [TestCase("042020", "052020", null)]
        [TestCase("042020", "032020", "This date must not be later than the projected apprenticeship training end date")]
        [TestCase("012019", "122020", "This date must be at least 3 months later than the employment start date")]
        [TestCase("032020", "122020", "This date must be at least 3 months later than the employment start date")]
        public void AndEmploymentEndDateIsNotLaterThanTrainingEndDate(string employmentEndDate, string trainingEndDate, string error)
        {
            var model = new EndDateViewModel
            {
                DeliveryModel = DeliveryModel.PortableFlexiJob,
                StartDate = "012020",
                EndDate = new MonthYearModel(trainingEndDate),
                EmploymentEndDate = new MonthYearModel(employmentEndDate),
            };
            
            var result = new EndDateViewModelValidator().TestValidate(model);

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
            var model = new EndDateViewModel
            {
                DeliveryModel = DeliveryModel.PortableFlexiJob,
                StartDate = "042022",
                EmploymentEndDate = new MonthYearModel("022023"),
                EndDate = new MonthYearModel("022022"),
            };
            
            var result = new EndDateViewModelValidator().TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                .WithErrorMessage("This date must not be before the end date for this employment")
                .WithoutErrorMessage("This date must be later than the employment start date");
        }

        private static void AssertValidationResult<T>(Expression<Func<EndDateViewModel, T>> property, EndDateViewModel instance, bool expectedValid)
        {
            var validator = new EndDateViewModelValidator();
            var result = validator.TestValidate(instance);

            if (expectedValid)
            {
                result.ShouldNotHaveValidationErrorFor(property);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(property);
            }
        }
    }
}