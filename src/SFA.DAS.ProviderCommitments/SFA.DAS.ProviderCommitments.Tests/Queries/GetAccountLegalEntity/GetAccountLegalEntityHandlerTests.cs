using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity;

namespace SFA.DAS.ProviderCommitments.Tests.Queries.GetAccountLegalEntity
{
    [TestFixture]
    public class GetAccountLegalEntityHandlerTests
    {
        [Test]
        public async Task Handle_WithValidRequest_ReturnsExpectedNames()
        {
            // Arrange
            const int accountLegalEntityId = 123;
            const string expectedAccountName = "ACME Corp";
            const string expectedEmployerName = "ACME Fireworks";

            var fixtures = new GetEmployerHandlerTestFixtures()
                .WithValidRequest()
                .WithLegalEntity(accountLegalEntityId, expectedAccountName, expectedEmployerName);

            // Act
            var response = await fixtures.InvokeHandler(accountLegalEntityId);

            // Assert
            Assert.AreEqual(expectedAccountName, response.AccountName);
            Assert.AreEqual(expectedEmployerName, response.LegalEntityName);
            Assert.AreEqual(accountLegalEntityId, response.AccountLegalEntityId);
        }

        [Test]
        public void Handle_WithInvalidRequests_ThrowsException()
        {
            // Arrange
            var fixtures = new GetEmployerHandlerTestFixtures()
                .WithInvalidRequest();

            // Act
            Assert.ThrowsAsync<ValidationException>(() => fixtures.InvokeHandler(123));
        }
    }


    internal class GetEmployerHandlerTestFixtures
    {
        public GetEmployerHandlerTestFixtures()
        {
            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();            
            ValidatorMock = new Mock<IValidator<GetAccountLegalEntityRequest>>();
        }

        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock { get; }
        public ICommitmentsApiClient CommitmentsApiClient => CommitmentsApiClientMock.Object;

        public Mock<IValidator<GetAccountLegalEntityRequest>> ValidatorMock { get; }
        public IValidator<GetAccountLegalEntityRequest> Validator => ValidatorMock.Object;

        public GetEmployerHandlerTestFixtures WithValidRequest()
        {
            return WithValidRequest(new ValidationFailure[0]);
        }

        public GetEmployerHandlerTestFixtures WithInvalidRequest()
        {
            return WithValidRequest(new []{new ValidationFailure("a property name", "the value is invalid") });
        }

        public GetEmployerHandlerTestFixtures WithLegalEntity(long accountLegalEntityId, string accountName,
            string legalEntityName)
        {
            CommitmentsApiClientMock
                .Setup(client => client.GetLegalEntity(accountLegalEntityId))
                .ReturnsAsync(() => new AccountLegalEntity
                {
                    AccountName = accountName,
                    LegalEntityName = legalEntityName
                });

            return this;
        }

        public GetAccountLegalEntityHandler CreateHandler()
        {
            return new GetAccountLegalEntityHandler(Validator, CommitmentsApiClient);
        }

        public GetAccountLegalEntityRequest BuildRequest(long accountLegalEntityId)
        {
            return new GetAccountLegalEntityRequest
            {
                EmployerAccountLegalEntityId =  accountLegalEntityId
            };
        }

        public Task<GetAccountLegalEntityResponse> InvokeHandler(long accountLegalEntityId)
        {
            var handler = CreateHandler();

            return handler.Handle(BuildRequest(accountLegalEntityId), CancellationToken.None);
        }

        private GetEmployerHandlerTestFixtures WithValidRequest(IEnumerable<ValidationFailure> errors)
        {
            // Validate<T>(instance, (IValidatorSelector) null, ruleSet)
            ValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext>()))
                .Returns(() => new ValidationResult(errors));

            return this;
        }
    }
}
