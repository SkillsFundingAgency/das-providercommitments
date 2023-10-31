using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class PriceViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new PriceViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new PriceViewModel { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(100000, true)]
        [TestCase(100001, false)]
        public void Validate_Price_ShouldBeValidated(int? price, bool expectedValid)
        {
            var model = new PriceViewModel { Price = price };
            AssertValidationResult(request => request.Price, model, expectedValid);
        }

        [TestCase(1000, null, false)]
        [TestCase(1000, 0, false)]
        [TestCase(1000, 1, true)]
        [TestCase(1000, 1000, true)]
        [TestCase(1000, 1001, false)]
        [TestCase(100000, 100000, true)]
        [TestCase(100000, 100001, false)]
        public void Validate_EmploymentPrice_ShouldBeValidated(int price, int? employmentPrice, bool expectedValid)
        {
            var model = new PriceViewModel { Price = price, EmploymentPrice = employmentPrice, DeliveryModel = DeliveryModel.PortableFlexiJob };
            AssertValidationResult(request => request.EmploymentPrice, model, expectedValid);
        }

        private static void AssertValidationResult<T>(Expression<Func<PriceViewModel, T>> property, PriceViewModel instance, bool expectedValid)
        {
            var validator = new PriceViewModelValidator();
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