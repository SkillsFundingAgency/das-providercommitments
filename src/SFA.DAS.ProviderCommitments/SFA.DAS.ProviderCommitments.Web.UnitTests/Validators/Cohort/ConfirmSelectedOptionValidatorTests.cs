using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    public class ConfirmSelectedOptionValidatorTests
    {
        [TestCase(null, false)]
        [TestCase("option1", true)]
        public void Validate_CohortReference_ShouldBeValidated(string selectedOption, bool expectedValid)
        {
            var model = new ViewSelectOptionsViewModel { SelectedOption = selectedOption };
            AssertValidationResult(request => request.SelectedOption, model, expectedValid);
        }
        
        private void AssertValidationResult<T>(Expression<Func<ViewSelectOptionsViewModel, T>> property, ViewSelectOptionsViewModel instance, bool expectedValid)
        {
            var validator = new ViewSelectOptionsViewModelValidator();

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