using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture()]
    public class ReservationsAddDraftApprenticeshipRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new ReservationsAddDraftApprenticeshipRequest { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }
        
        [TestCase(null, false)]
        [TestCase(1, true)]
        public void Validate_CohortId_ShouldBeValidated(int? cohortId, bool expectedValid)
        {
            var model = new ReservationsAddDraftApprenticeshipRequest { CohortId = cohortId };
            AssertValidationResult(request => request.CohortId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_CohortReference_ShouldBeValidated(string cohortReference, bool expectedValid)
        {
            var model = new ReservationsAddDraftApprenticeshipRequest { CohortReference = cohortReference };
            AssertValidationResult(request => request.CohortReference, model, expectedValid);
        }

        [TestCase("82019", true)]
        [TestCase("112019", true)]
        public void Validate_StartMonthYear_ShouldBeValidated(string startMonthYear, bool expectedValid)
        {
            var model = new ReservationsAddDraftApprenticeshipRequest { StartMonthYear = startMonthYear };
            AssertValidationResult(request => request.StartMonthYear, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<ReservationsAddDraftApprenticeshipRequest, T>> property, ReservationsAddDraftApprenticeshipRequest instance, bool expectedValid)
        {
            var validator = new ReservationsAddDraftApprenticeshipRequestValidator();
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