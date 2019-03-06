using System;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.Tests.Commands.CreateCohort
{
    [TestFixture]
    public class WhenIValidateCreateCohortRequest
    {
        private CreateCohortValidator _validator;
        private CreateCohortRequest _validRequest;
        private Func<ValidationResult> _act;

        [SetUp]
        public void Arrange()
        {
            _validRequest = new CreateCohortRequest
            {
                ProviderId = 123,
                EmployerAccountId = 456,
                LegalEntityId = "789"
            };

            _validator = new CreateCohortValidator();
            _act = () =>_validator.Validate(_validRequest);
        }

        [Test]
        public void ThenAValidRequestValidatesSuccessfully()
        {
            var result = _act();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ThenProviderIdIsRequired()
        {
            _validRequest.ProviderId = 0;
            var result = _act();
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenEmployerAccountIdIsRequired()
        {
            _validRequest.EmployerAccountId = 0;
            var result = _act();
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenLegalEntityIdIsRequired()
        {
            _validRequest.LegalEntityId = string.Empty;
            var result = _act();
            Assert.IsFalse(result.IsValid);
        }
    }
}
