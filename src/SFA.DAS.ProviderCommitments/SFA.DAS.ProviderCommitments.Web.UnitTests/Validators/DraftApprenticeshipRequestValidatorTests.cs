﻿using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class DraftApprenticeshipRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_CohortId_ShouldBeValidated(int cohortId, bool expectedValid)
        {
            var model = new DraftApprenticeshipRequest { CohortId = cohortId };
            AssertValidationResult(request => request.CohortId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new DraftApprenticeshipRequest { CohortReference = cohortReference };
            AssertValidationResult(request => request.CohortReference, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_DraftApprenticeshipId_ShouldBeValidated(int id, bool expectedValid)
        {
            var model = new DraftApprenticeshipRequest { DraftApprenticeshipId = id };
            AssertValidationResult(request => request.DraftApprenticeshipId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_DraftApprenticeshipHashedId_ShouldBeValidated(string reference, bool expectedValid)
        {
            var model = new DraftApprenticeshipRequest { DraftApprenticeshipHashedId = reference };
            AssertValidationResult(request => request.DraftApprenticeshipHashedId, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<DraftApprenticeshipRequest, T>> property, DraftApprenticeshipRequest instance, bool expectedValid)
        {
            var validator = new DraftApprenticeshipRequestValidator();
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