using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class FileUploadReviewViewModelValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(FileUploadReviewOption.ApproveAndSend, true)]
        [TestCase(FileUploadReviewOption.SaveButDontSend, true)]
        [TestCase(FileUploadReviewOption.UploadAmendedFile, true)]
        public void Validate_Selection_ShouldBeValidated(FileUploadReviewOption? selection, bool expectedValid)
        {
            var model = new FileUploadReviewViewModel { SelectedOption = selection };
            AssertValidationResult(request => request.SelectedOption, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<FileUploadReviewViewModel, T>> property, FileUploadReviewViewModel instance, bool expectedValid)
        {
            var validator = new FileUploadReviewViewModelValidator();
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
