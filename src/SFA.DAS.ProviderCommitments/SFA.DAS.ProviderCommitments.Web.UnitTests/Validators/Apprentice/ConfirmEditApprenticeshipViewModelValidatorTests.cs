using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class ConfirmEditApprenticeshipViewModelValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_ConfirmDeletion_ShouldBeValidated(bool? confirmDeletion, bool expectedValid)
        {
            var model = new ConfirmEditApprenticeshipViewModel() { ConfirmChanges = confirmDeletion };
            AssertValidationResult(request => request.ConfirmChanges, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<ConfirmEditApprenticeshipViewModel, T>> property, ConfirmEditApprenticeshipViewModel instance, bool expectedValid)
        {
            var validator = new ConfirmEditApprenticeshipViewModelValidator();
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
