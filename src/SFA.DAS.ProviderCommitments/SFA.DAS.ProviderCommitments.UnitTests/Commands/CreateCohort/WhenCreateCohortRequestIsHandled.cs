using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.UnitTests.Commands.CreateCohort
{
    [TestFixture]
    public class WhenCreateCohortRequestIsHandled
    {
        private CreateCohortHandlerFixture _fixture;
        
        [SetUp]
        public void Arrange()
        {
            _fixture = new CreateCohortHandlerFixture();
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
        
        private class CreateCohortHandlerFixture
        {
            private readonly CreateCohortHandler _handler;
            private readonly CreateCohortRequest _request;
            private readonly CommitmentsV2.Api.Types.Requests.CreateCohortRequest _apiRequest;
            private readonly CreateCohortRequest _requestClone;
            private CreateCohortResponse _result;
            private readonly CommitmentsV2.Api.Types.Responses.CreateCohortResponse _apiResponse;
            private readonly Mock<IValidator<CreateCohortRequest>> _validator;
            private readonly ValidationResult _validationResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IMapper<CreateCohortRequest, CommitmentsV2.Api.Types.Requests.CreateCohortRequest>> _mapper;


            public CreateCohortHandlerFixture()
            {
                var autoFixture = new Fixture();

                _request = autoFixture.Create<CreateCohortRequest>();
                _requestClone = TestHelper.Clone(_request);

                _validationResult = new ValidationResult();
                _validator = new Mock<IValidator<CreateCohortRequest>>();
                _validator.Setup(x => x.Validate(It.IsAny<CreateCohortRequest>()))
                    .Returns(_validationResult);

                _apiResponse = autoFixture.Create<CommitmentsV2.Api.Types.Responses.CreateCohortResponse>();
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.CreateCohort(It.IsAny<CommitmentsV2.Api.Types.Requests.CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_apiResponse);

                _mapper = new Mock<IMapper<CreateCohortRequest, CommitmentsV2.Api.Types.Requests.CreateCohortRequest>>();

                _apiRequest = autoFixture.Create<CommitmentsV2.Api.Types.Requests.CreateCohortRequest>();
                _mapper.Setup(m => m.Map(_requestClone))
                    .ReturnsAsync(_apiRequest);
                _handler = new CreateCohortHandler(_validator.Object, _apiClient.Object, _mapper.Object);
            }

            public async Task Act()
            {
                _result = await _handler.Handle(_requestClone, CancellationToken.None);
            }

            public CreateCohortHandlerFixture VerifyRequestWasValidated()
            {
                _validator.Verify(x => x.Validate(It.Is<CreateCohortRequest>(r => r ==_requestClone)));
                return this;
            }

            public CreateCohortHandlerFixture SetupValidationFailure()
            {
                _validationResult.Errors.Add(new ValidationFailure("Test", "TestError"));
                return this;
            }

            public CreateCohortHandlerFixture VerifyCohortWasCreated()
            {
                _apiClient.Verify(x =>
                    x.CreateCohort(It.Is<CommitmentsV2.Api.Types.Requests.CreateCohortRequest>(r =>
                        r.ProviderId == _apiRequest.ProviderId
                        && r.AccountLegalEntityId == _apiRequest.AccountLegalEntityId
                        && r.ReservationId == _apiRequest.ReservationId
                        && r.FirstName == _apiRequest.FirstName
                        && r.LastName == _apiRequest.LastName
                        && r.DateOfBirth == _apiRequest.DateOfBirth
                        && r.Uln == _apiRequest.Uln
                        && r.CourseCode == _apiRequest.CourseCode
                        && r.Cost == _apiRequest.Cost
                        && r.StartDate == _apiRequest.StartDate
                        && r.EndDate == _apiRequest.EndDate
                        && r.OriginatorReference == _apiRequest.OriginatorReference
                    ), It.IsAny<CancellationToken>()));
                return this;
            }

            public CreateCohortHandlerFixture VerifyCohortIdWasReturned()
            {
                Assert.AreEqual(_apiResponse.CohortId, _result.CohortId);
                return this;
            }
            
            public CreateCohortHandlerFixture VerifyCohortReferenceWasReturned()
            {
                Assert.AreEqual(_apiResponse.CohortReference, _result.CohortReference);
                return this;
            }
        }
    }
}
