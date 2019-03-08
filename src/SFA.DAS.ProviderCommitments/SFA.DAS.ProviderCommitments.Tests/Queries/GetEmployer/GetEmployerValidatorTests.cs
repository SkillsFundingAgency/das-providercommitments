using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;

namespace SFA.DAS.ProviderCommitments.Tests.Queries.GetEmployer
{
    [TestFixture]
    public class GetEmployerValidatorTests
    {
        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1234, true)]
        public void Valid_WithSpecifiedInput_ReturnsExpectedResults(long employerId, bool expectedIsValid)
        {
            // arrange
            var request = new GetEmployerRequest
            {
                EmployerAccountLegalEntityId = employerId
            };

            var validator = new GetEmployerValidator();

            var validationResult = validator.Validate(request);

            // act
            var actualIsValid = validationResult.IsValid;

            // assert
            Assert.AreEqual(expectedIsValid, actualIsValid);
        }
    }
}
