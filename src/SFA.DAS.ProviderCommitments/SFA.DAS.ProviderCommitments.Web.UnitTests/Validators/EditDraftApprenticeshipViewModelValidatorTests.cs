using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class EditDraftApprenticeshipViewModelValidatorTests
    {
        [TestCase(12, 12, 2000, true)]
        [TestCase(31, 2, 2000, false)]
        [TestCase(null, 2, 2000, false)]
        [TestCase(31, null, 2000, false)]
        [TestCase(31, 1, null, false)]
        [TestCase(null, null, null, true)]
        public void Validate_DoB_ShouldBeValidated(int? day, int? month, int? year, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipViewModel {BirthDay = day, BirthMonth = month, BirthYear = year};
            AssertValidationResult(request => request.DateOfBirth, model, expectedValid);
        }

        [TestCase(12, 12, 2000, true)]
        [TestCase(31, 2, 2000, false)]
        [TestCase(null, 2, 2000, false)]
        [TestCase(31, null, 2000, false)]
        [TestCase(31, 1, null, false)]
        [TestCase(null, null, null, true)]
        public void Validate_ActualStartDate_ShouldBeValidated(int? day, int? month, int? year, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipViewModel { ActualStartDay = day, ActualStartMonth = month, ActualStartYear = year };
            AssertValidationResult(request => request.ActualStartDate, model, expectedValid);
        }

        [TestCase(12, 2000, true)]
        [TestCase(13, 2000, false)]
        [TestCase(null, 2000, false)]
        [TestCase(1, null, false)]
        [TestCase(null, null, true)]
        public void Validate_StartDate_ShouldBeValidated(int? month, int? year, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipViewModel { StartMonth = month, StartYear = year };
            AssertValidationResult(request => request.StartDate, model, expectedValid);
        }

        [TestCase(12, 2000, true)]
        [TestCase(13, 2000, false)]
        [TestCase(null, 2000, false)]
        [TestCase(1, null, false)]
        [TestCase(null, null, true)]
        public void Validate_FinishDate_ShouldBeValidated(int? month, int? year, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipViewModel { IsOnFlexiPaymentPilot = false, EndMonth = month, EndYear = year };
            AssertValidationResult(request => request.EndDate, model, expectedValid);
        }
        private static void AssertValidationResult<T>(Expression<Func<EditDraftApprenticeshipViewModel, T>> property, EditDraftApprenticeshipViewModel instance, bool expectedValid)
        {
            var validator = new EditDraftApprenticeshipViewModelValidator();
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