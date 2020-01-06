using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;

namespace SFA.DAS.ProviderCommitments.UnitTests.Commands.CreateEmptyCohort
{
    [TestFixture]
    public class WhenCreateEmptyCohortRequestIsHandled
    {
        private CreateEmptyCohortHandlerFixture _fixture;
        
        [SetUp]
        public void Arrange()
        {
            _fixture = new CreateEmptyCohortHandlerFixture();
        }
        
        [Test]
        public async Task ThenTheRequestIsValidated()
        {
            await _fixture.Act();
            _fixture.VerifyRequestWasValidated();
        }

        [Test]
        public void ThenIfTheRequestIsInvalidThenAnExceptionIsThrown()
        {
            _fixture.SetupValidationFailure();
            Assert.ThrowsAsync<ValidationException>(() => _fixture.Act());
        }

        [Test]
        public async Task ThenTheCohortIsCreated()
        {
            await _fixture.Act();
            _fixture.VerifyCohortWasCreated();
        }

        [Test]
        public async Task ThenTheCohortIdIsReturned()
        {
            await _fixture.Act();
            _fixture.VerifyCohortIdWasReturned();
        }

        [Test]
        public async Task ThenTheCohortReferenceIsReturned()
        {
            await _fixture.Act();
            _fixture.VerifyCohortReferenceWasReturned();
        }
        
        private class CreateEmptyCohortHandlerFixture
        {
            private readonly CreateEmptyCohortHandler _handler;
            private readonly CreateEmptyCohortRequest _request;
            private readonly CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest _apiRequest;
            private CreateEmptyCohortResponse _result;
            private readonly CommitmentsV2.Api.Types.Responses.CreateCohortResponse _apiResponse;
            private readonly Mock<IValidator<CreateEmptyCohortRequest>> _validator;
            private readonly ValidationResult _validationResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IMapper<CreateEmptyCohortRequest, CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>> _mapper;

            public CreateEmptyCohortHandlerFixture()
            {
                var autoFixture = new Fixture();

                _request = autoFixture.Create<CreateEmptyCohortRequest>();

                _validationResult = new ValidationResult();
                _validator = new Mock<IValidator<CreateEmptyCohortRequest>>();
                _validator.Setup(x => x.Validate(It.IsAny<CreateEmptyCohortRequest>()))
                    .Returns(_validationResult);

                _apiResponse = autoFixture.Create<CommitmentsV2.Api.Types.Responses.CreateCohortResponse>();
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.CreateCohort(It.IsAny<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_apiResponse);

                _mapper = new Mock<IMapper<CreateEmptyCohortRequest, CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>>();

                _apiRequest = autoFixture.Create<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>();
                _mapper.Setup(m => m.Map(_request))
                    .ReturnsAsync(_apiRequest);
                _handler = new CreateEmptyCohortHandler(_validator.Object, _apiClient.Object, _mapper.Object);
            }

            public async Task Act()
            {
                _result = await _handler.Handle(_request, CancellationToken.None);
            }

            public CreateEmptyCohortHandlerFixture VerifyRequestWasValidated()
            {
                _validator.Verify(x => x.Validate(It.Is<CreateEmptyCohortRequest>(r => r ==_request)));
                return this;
            }

            public CreateEmptyCohortHandlerFixture SetupValidationFailure()
            {
                _validationResult.Errors.Add(new ValidationFailure("Test", "TestError"));
                return this;
            }

            public CreateEmptyCohortHandlerFixture VerifyCohortWasCreated()
            {
                _apiClient.Verify(x =>
                    x.CreateCohort(It.Is<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(r =>
                        r.ProviderId == _apiRequest.ProviderId
                        && r.AccountLegalEntityId == _apiRequest.AccountLegalEntityId
                    ), It.IsAny<CancellationToken>()));
                return this;
            }

            public CreateEmptyCohortHandlerFixture VerifyCohortIdWasReturned()
            {
                Assert.AreEqual(_apiResponse.CohortId, _result.CohortId);
                return this;
            }
            
            public CreateEmptyCohortHandlerFixture VerifyCohortReferenceWasReturned()
            {
                Assert.AreEqual(_apiResponse.CohortReference, _result.CohortReference);
                return this;
            }
        }
    }
}
