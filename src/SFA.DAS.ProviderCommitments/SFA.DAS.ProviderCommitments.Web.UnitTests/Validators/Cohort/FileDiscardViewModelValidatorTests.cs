using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    public class FileDiscardViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new FileDiscardViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_Confirm_ShouldBeValidated(bool? confirm, bool expectedValid)
        {
            var model = new FileDiscardViewModel { FileDiscardConfirmed = confirm };
            AssertValidationResult(request => request.FileDiscardConfirmed, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<FileDiscardViewModel, T>> property, FileDiscardViewModel instance, bool expectedValid)
        {
            var validator = new FileDiscardViewModelValidator();
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
