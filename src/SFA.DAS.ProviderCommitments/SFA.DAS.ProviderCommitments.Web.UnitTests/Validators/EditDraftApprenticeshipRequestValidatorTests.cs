using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class EditDraftApprenticeshipRequestValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(1, true)]
        public void Validate_CohortId_ShouldBeValidated(int? cohortId, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipRequest { CohortId = cohortId};
            AssertValidationResult(request => request.CohortId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipRequest { CohortReference = cohortReference };
            AssertValidationResult(request => request.CohortReference, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(1, true)]
        public void Validate_DraftApprenticeshipId_ShouldBeValidated(int? id, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipRequest { DraftApprenticeshipId = id };
            AssertValidationResult(request => request.DraftApprenticeshipId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_DraftApprenticeshipHashedId_ShouldBeValidated(string reference, bool expectedValid)
        {
            var model = new EditDraftApprenticeshipRequest { DraftApprenticeshipHashedId = reference };
            AssertValidationResult(request => request.DraftApprenticeshipHashedId, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<EditDraftApprenticeshipRequest, T>> property, EditDraftApprenticeshipRequest instance, bool expectedValid)
        {
            var validator = new EditDraftApprenticeshipRequestValidator();

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