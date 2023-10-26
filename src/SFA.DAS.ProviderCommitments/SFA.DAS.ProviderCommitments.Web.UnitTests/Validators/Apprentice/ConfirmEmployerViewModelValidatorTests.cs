using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class ConfirmEmployerViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Validate_ProviderId_ShouldBeValidated(int providerId, bool expectedValid)
        {
            var model = new ConfirmEmployerViewModel { ProviderId = providerId };
            AssertValidationResult(request => request.ProviderId, model, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_Confirm_ShouldBeValidated(bool? confirm, bool expectedValid)
        {
            var model = new ConfirmEmployerViewModel { Confirm = confirm };
            AssertValidationResult(request => request.Confirm, model, expectedValid);
        }

        [Test]
        public void Validate_Portable_Apprenticeship_Cannot_Move_To_Flexi_Job_Agency_Employer()
        {
            var model = new ConfirmEmployerViewModel { IsFlexiJobAgency = true, DeliveryModel = DeliveryModel.PortableFlexiJob, Confirm = true };
            AssertValidationResult(request => request.Confirm, model, false);
        }

        private static void AssertValidationResult<T>(Expression<Func<ConfirmEmployerViewModel, T>> property, ConfirmEmployerViewModel instance, bool expectedValid)
        {
            var validator = new ConfirmEmployerViewModelValidator();
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
