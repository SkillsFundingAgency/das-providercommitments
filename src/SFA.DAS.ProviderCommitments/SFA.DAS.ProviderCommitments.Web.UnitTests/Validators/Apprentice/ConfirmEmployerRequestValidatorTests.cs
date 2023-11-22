﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
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
