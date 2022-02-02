﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class FileUploadAmendedFileValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_Selection_ShouldBeValidated(bool? selection, bool expectedValid)
        {
            var model = new FileUploadAmendedFileViewModel { Confirm = selection };
            AssertValidationResult(request => request.Confirm, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<FileUploadAmendedFileViewModel, T>> property, FileUploadAmendedFileViewModel instance, bool expectedValid)
        {
            var validator = new FileUploadAmendedFileViewModelValidator();

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