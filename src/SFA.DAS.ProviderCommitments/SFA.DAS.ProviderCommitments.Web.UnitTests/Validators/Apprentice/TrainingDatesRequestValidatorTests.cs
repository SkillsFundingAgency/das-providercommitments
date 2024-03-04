﻿using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class TrainingDatesRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new TrainingDatesRequest { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new TrainingDatesRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<TrainingDatesRequest, T>> property, TrainingDatesRequest instance, bool expectedValid)
        {
            var validator = new TrainingDatesRequestValidator();
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