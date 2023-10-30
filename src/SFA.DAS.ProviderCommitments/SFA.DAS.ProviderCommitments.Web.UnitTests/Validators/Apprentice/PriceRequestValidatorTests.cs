using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class PriceRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new PriceRequest { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new PriceRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<PriceRequest, T>> property, PriceRequest instance, bool expectedValid)
        {
            var validator = new PriceRequestValidator();
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