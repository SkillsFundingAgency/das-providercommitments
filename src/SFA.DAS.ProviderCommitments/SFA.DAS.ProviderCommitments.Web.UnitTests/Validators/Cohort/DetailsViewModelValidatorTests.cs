using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class DetailsViewModelValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(CohortDetailsOptions.Send, true)]
        [TestCase(CohortDetailsOptions.Approve, true)]
        public void Validate_Selection_ShouldBeValidated(CohortDetailsOptions? selection, bool expectedValid)
        {
            var model = new DetailsViewModel { Selection = selection };
            AssertValidationResult(request => request.Selection, model, expectedValid);
        }

        [TestCase(true, CohortDetailsOptions.Send, true)]
        [TestCase(true, CohortDetailsOptions.Approve, true)]
        [TestCase(true, CohortDetailsOptions.ApprenticeRequest, true)]
        [TestCase(false, CohortDetailsOptions.Send, false)]
        [TestCase(false, CohortDetailsOptions.Approve, false)]
        [TestCase(false, CohortDetailsOptions.ApprenticeRequest, true)]
        public void Validate_RplVerified_ShouldBeValidated(bool rplVerified, CohortDetailsOptions? selection,  bool expectedValid)
        {
            var model = new DetailsViewModel { RplVerified = rplVerified, Selection = selection };
            AssertValidationResult(request => request.RplVerified, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<DetailsViewModel, T>> property, DetailsViewModel instance, bool expectedValid)
        {
            var validator = new DetailsViewModelValidator();
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
