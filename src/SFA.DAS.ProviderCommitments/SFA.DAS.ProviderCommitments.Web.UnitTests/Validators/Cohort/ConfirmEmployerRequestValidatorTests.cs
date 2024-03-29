﻿using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
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

        private static void AssertValidationResult<T>(Expression<Func<ConfirmEmployerRequest, T>> property, ConfirmEmployerRequest instance, bool expectedValid)
        {
            var validator = new ConfirmEmployerRequestValidator();
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
