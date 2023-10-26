using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class DeleteConfirmationViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new DeleteConfirmationViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new DeleteConfirmationViewModel() { DraftApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.DraftApprenticeshipHashedId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_DeleteConfirmed_ShouldBeValidated(bool? deleteConfirmed, bool expectedValid)
        {
            var model = new DeleteConfirmationViewModel { DeleteConfirmed = deleteConfirmed };
            AssertValidationResult(request => request.DeleteConfirmed, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<DeleteConfirmationViewModel, T>> property, DeleteConfirmationViewModel instance, bool expectedValid)
        {
            var validator = new DeleteConfirmationViewModelValidator();
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
