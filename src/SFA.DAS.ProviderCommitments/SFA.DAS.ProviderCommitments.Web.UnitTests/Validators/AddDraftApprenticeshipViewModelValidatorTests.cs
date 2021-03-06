﻿using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class AddDraftApprenticeshipViewModelValidatorTests
    {
        [TestCase(12, 12, 2000, true)]
        [TestCase(31, 2, 2000, false)]
        [TestCase(null, 2, 2000, false)]
        [TestCase(31, null, 2000, false)]
        [TestCase(31, 1, null, false)]
        [TestCase(null, null, null, true)]
        public void Validate_DoB_ShouldBeValidated(int? day, int? month, int? year, bool expectedValid)
        {
            var model = new AddDraftApprenticeshipViewModel {BirthDay = day, BirthMonth = month, BirthYear = year};
            AssertValidationResult(request => request.DateOfBirth, model, expectedValid);
        }

        [TestCase(12, 2000, true)]
        [TestCase(13, 2000, false)]
        [TestCase(null, 2000, false)]
        [TestCase(1, null, false)]
        [TestCase(null, null, true)]
        public void Validate_StartDate_ShouldBeValidated(int? month, int? year, bool expectedValid)
        {
            var model = new AddDraftApprenticeshipViewModel { StartMonth = month, StartYear = year };
            AssertValidationResult(request => request.StartDate, model, expectedValid);
        }

        [TestCase(12, 2000, true)]
        [TestCase(13, 2000, false)]
        [TestCase(null, 2000, false)]
        [TestCase(1, null, false)]
        [TestCase(null, null, true)]
        public void Validate_FinishDate_ShouldBeValidated(int? month, int? year, bool expectedValid)
        {
            var model = new AddDraftApprenticeshipViewModel { EndMonth = month, EndYear = year };
            AssertValidationResult(request => request.EndDate, model, expectedValid);
        }
        private void AssertValidationResult<T>(Expression<Func<AddDraftApprenticeshipViewModel, T>> property, AddDraftApprenticeshipViewModel instance, bool expectedValid)
        {
            var validator = new AddDraftApprenticeshipViewModelValidator();

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