using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class DatesRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new DatesRequest { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void ThenEmployerAccountLegalEntityPublicHashedIdIsValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new DatesRequest { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };

            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(102, true)]
        public void ThenAccountLegalEntityIdIsValidated(long accountLegalEntityId, bool expectedValid)
        {
            var model = new DatesRequest { AccountLegalEntityId = accountLegalEntityId };
            AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new DatesRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<DatesRequest, T>> property, DatesRequest instance, bool expectedValid)
        {
            var validator = new DatesRequestValidator();

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