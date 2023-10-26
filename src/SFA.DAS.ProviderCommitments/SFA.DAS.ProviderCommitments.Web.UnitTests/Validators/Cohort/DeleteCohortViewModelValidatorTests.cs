using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class DeleteCohortViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new DeleteCohortViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_Confirm_ShouldBeValidated(bool? confirm, bool expectedValid)
        {
            var model = new DeleteCohortViewModel { Confirm = confirm };
            AssertValidationResult(request => request.Confirm, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new DeleteCohortViewModel { CohortReference = cohortReference };
            AssertValidationResult(request => request.CohortReference, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<DeleteCohortViewModel, T>> property, DeleteCohortViewModel instance, bool expectedValid)
        {
            var validator = new DeleteCohortViewModelValidator();
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
