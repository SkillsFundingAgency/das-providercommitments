using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class ConfirmEmployerViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new ConfirmEmployerViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_Confirm_ShouldBeValidated(bool? confirm, bool expectedValid)
        {
            var model = new ConfirmEmployerViewModel { Confirm = confirm };
            AssertValidationResult(request => request.Confirm, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_EmployerAccountLegalEntityPublicHashedId_ShouldBeValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new ConfirmEmployerViewModel { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };
            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        //[TestCase(0, false)]
        //[TestCase(102, true)]
        //public void Validate_AccountLegalEntityId_ShouldBeValidated(long accountLegalEntityId, bool expectedValid)
        //{
        //    var model = new ConfirmEmployerViewModel { AccountLegalEntityId = accountLegalEntityId };
        //    AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
        //}

        private void AssertValidationResult<T>(Expression<Func<ConfirmEmployerViewModel, T>> property, ConfirmEmployerViewModel instance, bool expectedValid)
        {
            var validator = new ConfirmEmployerViewModelValidator();

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
