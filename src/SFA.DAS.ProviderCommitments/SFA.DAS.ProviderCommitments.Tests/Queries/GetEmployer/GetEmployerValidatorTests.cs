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
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("   ", false)]
        [TestCase("1234", true)]
        public void Valid_WithSpecifiedInput_ReturnsExpectedResults(string employerId, bool expectedIsValid)
        {
            // arrange
            var request = new GetEmployerRequest
            {
                EmployerAccountPublicHashedId = employerId
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
