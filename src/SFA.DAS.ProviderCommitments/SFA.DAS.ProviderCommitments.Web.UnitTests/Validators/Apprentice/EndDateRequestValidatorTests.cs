using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    public class EndDateRequestValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new EndDateRequest { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new EndDateRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
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
            var model = new EndDateRequest { StartDate = startDate };
            AssertValidationResult(request => request.StartDate, model, expectedValid);
        }

        private void AssertValidationResult<T>(Expression<Func<EndDateRequest, T>> property, EndDateRequest instance, bool expectedValid)
        {
            var validator = new EndDateRequestValidator();

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