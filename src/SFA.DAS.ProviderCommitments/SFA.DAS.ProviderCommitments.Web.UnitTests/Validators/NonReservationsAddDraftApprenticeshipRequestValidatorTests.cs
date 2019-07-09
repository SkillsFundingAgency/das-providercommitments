using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class NonReservationsAddDraftApprenticeshipRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new NonReservationsAddDraftApprenticeshipRequest { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }
        
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
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new NonReservationsAddDraftApprenticeshipRequest { CohortReference = cohortReference };
            AssertValidationResult(request => request.CohortReference, model, expectedValid);
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