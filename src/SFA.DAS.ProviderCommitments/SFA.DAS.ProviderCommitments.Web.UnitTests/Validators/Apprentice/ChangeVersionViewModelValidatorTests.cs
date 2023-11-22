using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class ChangeVersionViewModelValidatorTests
    {
        [TestCase("1.0", true)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void ValidateSelectedVersion(string version, bool expectedValid)
        {
            var viewModel = new ChangeVersionViewModel { SelectedVersion = version };

            AssertValidationResult(x => x.SelectedVersion, viewModel, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<ChangeVersionViewModel, T>> property, ChangeVersionViewModel instance, bool expectedValid)
        {
            var validator = new ChangeVersionViewModelValidator();
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
