using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.OverlappingTrainingDates
{
    [TestFixture]
    public class ChangeOfEmployerNotifiedViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new ChangeOfEmployerNotifiedViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(NextAction.ViewAllCohorts, true)]
        [TestCase(NextAction.ViewDashBoard, true)]
        public void Validate_NextAction_ShouldBeValidated(NextAction? action, bool expectedValid)
        {
            var model = new ChangeOfEmployerNotifiedViewModel { NextAction = action };
            AssertValidationResult(request => request.NextAction, model, expectedValid);
        }
       
        private void AssertValidationResult<T>(Expression<Func<ChangeOfEmployerNotifiedViewModel, T>> property, ChangeOfEmployerNotifiedViewModel instance, bool expectedValid)
        {
            var validator = new ChangeOfEmployerNotifiedViewModelValidator();

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
