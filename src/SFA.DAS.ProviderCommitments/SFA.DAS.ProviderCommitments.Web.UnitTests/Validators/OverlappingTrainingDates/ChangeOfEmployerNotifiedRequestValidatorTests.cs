﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.OverlappingTrainingDates
{
    [TestFixture]
    public class ChangeOfEmployerNotifiedRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new ChangeOfEmployerNotifiedRequest { ProviderId = providerId };
            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<ChangeOfEmployerNotifiedRequest, T>> property,
            ChangeOfEmployerNotifiedRequest instance, bool expectedValid)
        {
            var validator = new ChangeOfEmployerNotifiedRequestValidator();

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