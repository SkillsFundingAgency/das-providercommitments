using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class ReviewApprenticeViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new ReviewApprenticeViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new ReviewApprenticeViewModel { CohortRef = cohortReference };
            AssertValidationResult(request => request.CohortRef, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<ReviewApprenticeViewModel, T>> property, ReviewApprenticeViewModel instance, bool expectedValid)
        {
            var validator = new ReviewApprenticeViewModelValidator();

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
