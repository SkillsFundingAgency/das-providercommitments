using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.Validators;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture]
    public class ConfirmEmployerRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new ConfirmEmployerRequest { ProviderId = providerId };
            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_EmployerAccountLegalEntityPublicHashedId_ShouldBeValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new ConfirmEmployerRequest { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };
            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(102, true)]
        public void Validate_AccountLegalEntityId_ShouldBeValidated(long accountLegalEntityId, bool expectedValid)
        {
            var model = new ConfirmEmployerRequest { AccountLegalEntityId = accountLegalEntityId };
            AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<ConfirmEmployerRequest, T>> property, ConfirmEmployerRequest instance, bool expectedValid)
        {
            var validator = new ConfirmEmployerRequestValidator();

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
