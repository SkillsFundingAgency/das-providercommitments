using System;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;

namespace SFA.DAS.ProviderCommitments.UnitTests.Commands.CreateEmptyCohort
{
    [TestFixture]
    public class WhenCreateEmptyCohortRequestIsValidated
    {
        private CreateEmptyCohortValidator _validator;
        private CreateEmptyCohortRequest _validRequest;
        private Func<ValidationResult> _act;

        [SetUp]
        public void Arrange()
        {
            _validRequest = new CreateEmptyCohortRequest
            {
                ProviderId = 123,
                AccountLegalEntityId = 456,
            };

            _validator = new CreateEmptyCohortValidator();
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
            _validRequest.AccountLegalEntityId = 0;
            var result = _act();
            Assert.IsFalse(result.IsValid);
        }
    }
}
