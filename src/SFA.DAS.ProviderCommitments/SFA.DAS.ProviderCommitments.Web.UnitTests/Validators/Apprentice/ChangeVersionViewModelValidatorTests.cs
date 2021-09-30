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

        private void AssertValidationResult<T>(Expression<Func<ChangeVersionViewModel, T>> property, ChangeVersionViewModel instance, bool expectedValid)
        {
            var validator = new ChangeVersionViewModelValidator();

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
