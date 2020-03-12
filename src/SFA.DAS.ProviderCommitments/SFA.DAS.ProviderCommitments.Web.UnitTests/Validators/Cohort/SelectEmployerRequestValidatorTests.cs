using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class SelectEmployerRequestValidatorTests 
    {

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new SelectEmployerRequest {ProviderId = providerId};
            AssertValidationResult(x => x.ProviderId, request, expectedValid);
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
