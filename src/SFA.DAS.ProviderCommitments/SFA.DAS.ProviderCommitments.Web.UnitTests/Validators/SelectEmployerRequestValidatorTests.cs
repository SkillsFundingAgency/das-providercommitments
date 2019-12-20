using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.Validators;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators
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
