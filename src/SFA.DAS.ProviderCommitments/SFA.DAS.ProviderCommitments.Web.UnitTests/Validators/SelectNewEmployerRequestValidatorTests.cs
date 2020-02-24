using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Requests.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators;
using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
{
    [TestFixture]
    public class SelectNewEmployerRequestValidatorTests
    {

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new SelectNewEmployerRequest { ProviderId = providerId };
            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenApprenticeshipIdIsValidated(long apprenticeshipId, bool expectedValid)
        {
            var request = new SelectNewEmployerRequest { ApprenticeshipId = apprenticeshipId };
            AssertValidationResult(x => x.ApprenticeshipId, request, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<SelectNewEmployerRequest, T>> property, SelectNewEmployerRequest instance, bool expectedValid)
        {
            var validator = new SelectNewEmployerRequestValidator();

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
