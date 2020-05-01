using FluentValidation.TestHelper;
using NUnit.Framework;
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

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void Validate_EmployerAccountLegalEntityPublicHashedId_ShouldBeValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new PriceRequest { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };

            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new PriceRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XXXXXXX", false)]
        [TestCase("1220", false)]
        [TestCase("12002", true)]
        [TestCase("012002", true)]
        public void Validate_StartDate_ShouldBeValidated(string startDate, bool expectedValid)
        {
            var model = new PriceRequest { StartDate = startDate };
            AssertValidationResult(request => request.StartDate, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XXXXXXX", false)]
        [TestCase("1220", false)]
        [TestCase("12002", true)]
        [TestCase("012002", true)]
        public void Validate_EndDate_ShouldBeValidated(string endDate, bool expectedValid)
        {
            var model = new PriceRequest { EndDate = endDate };
            AssertValidationResult(request => request.EndDate, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<PriceRequest, T>> property, PriceRequest instance, bool expectedValid)
        {
            var validator = new PriceRequestValidator();

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