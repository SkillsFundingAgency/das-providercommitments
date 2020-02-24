using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Requests.Apprentice;
using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class SelectEmployerRequestValidatorTests
    {

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new SelectEmployerRequest { ProviderId = providerId };
            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenApprenticeshipIdIsValidated(long apprenticeshipId, bool expectedValid)
        {
            var request = new SelectEmployerRequest { ApprenticeshipId = apprenticeshipId };
            AssertValidationResult(x => x.ApprenticeshipId, request, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<SelectEmployerRequest, T>> property, SelectEmployerRequest instance, bool expectedValid)
        {
            var validator = new SelectEmployerRequestValidator();

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
