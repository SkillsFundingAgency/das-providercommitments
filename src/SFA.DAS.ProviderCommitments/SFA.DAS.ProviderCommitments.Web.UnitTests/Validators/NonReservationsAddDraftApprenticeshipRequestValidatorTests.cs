using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class NonReservationsAddDraftApprenticeshipRequestValidatorTests
    {
        [TestCase(null, false)]
        [TestCase(1, true)]
        public void Validate_CohortId_ShouldBeValidated(int? cohortId, bool expectedValid)
        {
            var model = new NonReservationsAddDraftApprenticeshipRequest { CohortId = cohortId};
            AssertValidationResult(request => request.CohortId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortPublicHashedId_ShouldBeValidated(string cohortPublicHashedId, bool expectedValid)
        {
            var model = new NonReservationsAddDraftApprenticeshipRequest { CohortPublicHashedId = cohortPublicHashedId };
            AssertValidationResult(request => request.CohortPublicHashedId, model, expectedValid);
        }


        private void AssertValidationResult<T>(Expression<Func<NonReservationsAddDraftApprenticeshipRequest, T>> property, NonReservationsAddDraftApprenticeshipRequest instance, bool expectedValid)
        {
            var validator = new NonReservationsAddDraftApprenticeshipRequestValidator();

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