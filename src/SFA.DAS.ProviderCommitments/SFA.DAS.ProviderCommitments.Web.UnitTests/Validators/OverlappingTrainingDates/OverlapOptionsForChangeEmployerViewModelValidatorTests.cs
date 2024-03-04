using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.OverlappingTrainingDates
{
    [TestFixture]
    public class OverlapOptionsForChangeEmployerViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new OverlapOptionsForChangeEmployerViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }


        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ApprenticeshipId_ShouldBeValidated(int apprenticeshipId, bool expectedValid)
        {
            var model = new OverlapOptionsForChangeEmployerViewModel { ApprenticeshipId = apprenticeshipId };
            AssertValidationResult(request => request.ApprenticeshipId, model, expectedValid);
        }


        [TestCase(null, false)]
        [TestCase(OverlapOptions.CompleteActionLater, true)]
        [TestCase(OverlapOptions.ContactTheEmployer, true)]
        [TestCase(OverlapOptions.SendStopRequest, true)]
        public void Validate_Selection_ShouldBeValidated(OverlapOptions? selection, bool expectedValid)
        {
            var model = new OverlapOptionsForChangeEmployerViewModel { OverlapOptions = selection };
            AssertValidationResult(request => request.OverlapOptions, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<OverlapOptionsForChangeEmployerViewModel, T>> property, OverlapOptionsForChangeEmployerViewModel instance, bool expectedValid)
        {
            var validator = new OverlapOptionsForChangeEmployerViewModelValidator();
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
