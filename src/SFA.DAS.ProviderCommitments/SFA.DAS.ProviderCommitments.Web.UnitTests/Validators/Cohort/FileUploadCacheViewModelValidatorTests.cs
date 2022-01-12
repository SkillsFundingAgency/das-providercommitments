using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class FileUploadCacheViewModelValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(FileUploadCacheOption.ApproveAndSend, true)]
        [TestCase(FileUploadCacheOption.SaveButDontSend, true)]
        [TestCase(FileUploadCacheOption.UploadAmendedFile, true)]
        public void Validate_Selection_ShouldBeValidated(FileUploadCacheOption? selection, bool expectedValid)
        {
            var model = new FileUploadCacheViewModel { SelectedOption = selection };
            AssertValidationResult(request => request.SelectedOption, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<FileUploadCacheViewModel, T>> property, FileUploadCacheViewModel instance, bool expectedValid)
        {
            var validator = new FileUploadCacheViewModelValidator();

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
