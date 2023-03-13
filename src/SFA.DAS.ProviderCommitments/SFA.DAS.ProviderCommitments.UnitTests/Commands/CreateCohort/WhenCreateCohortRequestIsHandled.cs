using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using CreateCohortResponse = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortResponse;

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
        
        
        [Test]
        public async Task ThenHasStandardOptionsIsTrueIfSingleApprenticeWithOptions()
        {   
            await _fixture.Act();

            _fixture.VerifyHasOptionsValue(true);
        }

        [Test]
        public async Task ThenHasStandardOptionsIsFalseIfMultipleApprentices()
        {
            _fixture.ReturnMultipleApprenticeships();
            
            await _fixture.Act();

            _fixture.VerifyHasOptionsValue(false);
        }
        
        
        [Test]
        public async Task ThenHasStandardOptionsIsFalseIfSingleApprenticeWithNoOptions()
        {
            _fixture.ReturnMultipleApprenticeships();
            
            await _fixture.Act();

            _fixture.VerifyHasOptionsValue(false);
        }
        
        private class CreateCohortHandlerFixture
        {
            private readonly CreateCohortHandler _handler;
            private readonly CreateCohortRequest _request;
            private readonly CreateCohortApimRequest _apiRequest;
            private readonly CreateCohortRequest _requestClone;
            private CreateCohortResponse _result;
            private readonly ProviderCommitments.Infrastructure.OuterApi.Responses.CreateCohortResponse _apiResponse;
            private readonly Mock<IValidator<CreateCohortRequest>> _validator;
            private readonly ValidationResult _validationResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IMapper<CreateCohortRequest, CreateCohortApimRequest>> _mapper;
            private readonly Mock<IOuterApiService> _outerApi;
            private readonly DraftApprenticeshipDto _draftResponse;
            private Fixture _autoFixture;


            public CreateCohortHandlerFixture()
            {
                _autoFixture = new Fixture();

                _request = _autoFixture.Create<CreateCohortRequest>();
                _requestClone = TestHelper.Clone(_request);

                _draftResponse = _autoFixture.Build<DraftApprenticeshipDto>().Create();
                
                var getDraftApprenticeshipsResponse = new GetDraftApprenticeshipsResponse
                {
                    DraftApprenticeships = new List<DraftApprenticeshipDto>
                    {
                        _draftResponse
                    }
                };

                var getDraftApprenticeshipResponse = _autoFixture.Build<GetDraftApprenticeshipResponse>()
                    .With(c=>c.HasStandardOptions, true)
                    .Create();
                
                
                _validationResult = new ValidationResult();
                _validator = new Mock<IValidator<CreateCohortRequest>>();
                _validator.Setup(x => x.Validate(It.IsAny<CreateCohortRequest>()))
                    .Returns(_validationResult);

                _apiResponse = _autoFixture.Create<ProviderCommitments.Infrastructure.OuterApi.Responses.CreateCohortResponse>();
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.GetDraftApprenticeships(_apiResponse.CohortId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(getDraftApprenticeshipsResponse);
                _apiClient.Setup(x => x.GetDraftApprenticeship(_apiResponse.CohortId, _draftResponse.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(getDraftApprenticeshipResponse);
                
                _mapper = new Mock<IMapper<CreateCohortRequest, CreateCohortApimRequest>>();

                _apiRequest = _autoFixture.Create<CreateCohortApimRequest>();
                _mapper.Setup(m => m.Map(_requestClone))
                    .ReturnsAsync(_apiRequest);

                _outerApi = new Mock<IOuterApiService>();
                _outerApi.Setup(x => x.CreateCohort(It.IsAny<CreateCohortApimRequest>())).ReturnsAsync(_apiResponse);

                _handler = new CreateCohortHandler(_validator.Object, _apiClient.Object, _outerApi.Object, _mapper.Object);
            }

            public CreateCohortHandlerFixture ReturnMultipleApprenticeships()
            {
                var getDraftApprenticeshipsResponse = _autoFixture.Create<GetDraftApprenticeshipsResponse>();
                _apiClient.Setup(x => x.GetDraftApprenticeships(_apiResponse.CohortId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(getDraftApprenticeshipsResponse);
                return this;
            }
            
            
            public CreateCohortHandlerFixture ReturnSingleApprenticeshipNoOptions()
            {
                var getDraftApprenticeshipResponse = _autoFixture.Build<GetDraftApprenticeshipResponse>()
                    .With(c=>c.HasStandardOptions, false)
                    .Create();
                _apiClient.Setup(x => x.GetDraftApprenticeship(_apiResponse.CohortId, _draftResponse.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(getDraftApprenticeshipResponse);
                return this;
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
                _outerApi.Verify(x =>
                    x.CreateCohort(It.Is<CreateCohortApimRequest>(r =>
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
                        && r.ActualStartDate == _apiRequest.ActualStartDate
                        && r.EndDate == _apiRequest.EndDate
                        && r.OriginatorReference == _apiRequest.OriginatorReference
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

            public CreateCohortHandlerFixture VerifyHasOptionsValue(bool hasOptions)
            {
                if (hasOptions)
                {
                    Assert.IsNotNull(_result.DraftApprenticeshipId);    
                }
                else
                {
                    Assert.IsNull(_result.DraftApprenticeshipId);
                }
                
                return this;
            }
        }
    }
}
