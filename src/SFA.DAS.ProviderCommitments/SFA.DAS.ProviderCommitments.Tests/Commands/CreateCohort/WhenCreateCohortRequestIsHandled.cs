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
        private CreateCohortHandler _handler;
        private CreateCohortRequest _request;
        private CreateCohortRequest _requestClone;
        private CommitmentsV2.Api.Types.CreateCohortResponse _apiResponse;
        private Mock<IValidator<CreateCohortRequest>> _validator;
        private ValidationResult _validationResult;
        private Mock<ICommitmentsApiClient> _apiClient;
        private Func<Task<CreateCohortResponse>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _request = fixture.Create<CreateCohortRequest>();
            _requestClone = TestHelper.Clone(_request);

            _validationResult = new ValidationResult();
            _validator = new Mock<IValidator<CreateCohortRequest>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateCohortRequest>()))
                .Returns(_validationResult);

            _apiResponse = fixture.Create<CommitmentsV2.Api.Types.CreateCohortResponse>();
            _apiClient = new Mock<ICommitmentsApiClient>();
            _apiClient.Setup(x => x.CreateCohort(It.IsAny<CommitmentsV2.Api.Types.CreateCohortRequest>()))
                .ReturnsAsync(_apiResponse);

            _handler = new CreateCohortHandler(_validator.Object, _apiClient.Object);

            _act = () => _handler.Handle(_requestClone, CancellationToken.None);
        }

        [Test]
        public void ThenTheRequestIsValidated()
        {
            _act();
            _validator.Verify(x => x.Validate(It.Is<CreateCohortRequest>(r => r ==_requestClone)));
        }

        [Test]
        public void ThenIfTheRequestIsInvalidThenAnExceptionIsThrown()
        {
            _validationResult.Errors.Add(new ValidationFailure("Test", "TestError"));
            Assert.ThrowsAsync<ValidationException>(() => _act());
        }

        [Test]
        public void ThenTheCohortIsCreated()
        {
            _act();
            _apiClient.Verify(x =>
                x.CreateCohort(It.Is<CommitmentsV2.Api.Types.CreateCohortRequest>(r =>
                    r.Cohort.ProviderId == _requestClone.ProviderId
                    && r.Cohort.EmployerAccountId == _requestClone.EmployerAccountId
                    && r.Cohort.LegalEntityId == _requestClone.LegalEntityId
                    && r.DraftApprenticeship.ReservationId == _requestClone.ReservationId
                    && r.DraftApprenticeship.FirstName == _requestClone.FirstName
                    && r.DraftApprenticeship.LastName == _requestClone.LastName
                    && r.DraftApprenticeship.DateOfBirth == _requestClone.DateOfBirth
                    && r.DraftApprenticeship.ULN == _requestClone.UniqueLearnerNumber
                    && r.DraftApprenticeship.CourseCode == _requestClone.CourseCode
                    && r.DraftApprenticeship.Cost == _requestClone.Cost
                    && r.DraftApprenticeship.StartDate == _requestClone.StartDate
                    && r.DraftApprenticeship.EndDate == _requestClone.EndDate
                    && r.DraftApprenticeship.OriginatorReference == _requestClone.OriginatorReference
                )));
        }

        [Test]
        public async Task ThenTheCohortIdIsReturned()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.CohortId, result.CohortId);
        }

        [Test]
        public async Task ThenTheCohortReferenceIsReturned()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.CohortReference, result.CohortReference);
        }
    }
}
