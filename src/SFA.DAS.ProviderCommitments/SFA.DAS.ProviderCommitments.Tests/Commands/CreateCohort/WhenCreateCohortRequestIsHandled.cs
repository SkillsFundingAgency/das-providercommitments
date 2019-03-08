using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.Tests.Commands.CreateCohort
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
            private readonly CreateCohortRequest _requestClone;
            private CreateCohortResponse _result;
            private readonly CommitmentsV2.Api.Types.CreateCohortResponse _apiResponse;
            private readonly Mock<IValidator<CreateCohortRequest>> _validator;
            private readonly ValidationResult _validationResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            
            public CreateCohortHandlerFixture()
            {
                var autoFixture = new Fixture();

                _request = autoFixture.Create<CreateCohortRequest>();
                _requestClone = TestHelper.Clone(_request);

                _validationResult = new ValidationResult();
                _validator = new Mock<IValidator<CreateCohortRequest>>();
                _validator.Setup(x => x.Validate(It.IsAny<CreateCohortRequest>()))
                    .Returns(_validationResult);

                _apiResponse = autoFixture.Create<CommitmentsV2.Api.Types.CreateCohortResponse>();
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.CreateCohort(It.IsAny<CommitmentsV2.Api.Types.CreateCohortRequest>()))
                    .ReturnsAsync(_apiResponse);

                _handler = new CreateCohortHandler(_validator.Object, _apiClient.Object);
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
                    x.CreateCohort(It.Is<CommitmentsV2.Api.Types.CreateCohortRequest>(r =>
                        r.Cohort.ProviderId == _request.ProviderId
                        && r.Cohort.EmployerAccountId == _request.EmployerAccountId
                        && r.Cohort.LegalEntityId == _request.LegalEntityId
                        && r.DraftApprenticeship.ReservationId == _request.ReservationId
                        && r.DraftApprenticeship.FirstName == _request.FirstName
                        && r.DraftApprenticeship.LastName == _request.LastName
                        && r.DraftApprenticeship.DateOfBirth == _request.DateOfBirth
                        && r.DraftApprenticeship.ULN == _request.UniqueLearnerNumber
                        && r.DraftApprenticeship.CourseCode == _request.CourseCode
                        && r.DraftApprenticeship.Cost == _request.Cost
                        && r.DraftApprenticeship.StartDate == _request.StartDate
                        && r.DraftApprenticeship.EndDate == _request.EndDate
                        && r.DraftApprenticeship.OriginatorReference == _request.OriginatorReference
                    )));
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
