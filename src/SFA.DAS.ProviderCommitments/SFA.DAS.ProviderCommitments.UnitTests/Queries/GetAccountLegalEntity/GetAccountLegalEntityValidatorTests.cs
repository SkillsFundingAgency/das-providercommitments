using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetAccountLegalEntity
{
    [TestFixture]
    public class GetAccountLegalEntityValidatorTests
    {
        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1234, true)]
        public void Valid_WithSpecifiedInput_ReturnsExpectedResults(long employerId, bool expectedIsValid)
        {
            // arrange
            var request = new GetAccountLegalEntityRequest
            {
                EmployerAccountLegalEntityId = employerId
            };

            var validator = new GetAccountLegalEntityValidator();

            var validationResult = validator.Validate(request);

            // act
            var actualIsValid = validationResult.IsValid;

            // assert
            Assert.AreEqual(expectedIsValid, actualIsValid);
        }
    }
}
