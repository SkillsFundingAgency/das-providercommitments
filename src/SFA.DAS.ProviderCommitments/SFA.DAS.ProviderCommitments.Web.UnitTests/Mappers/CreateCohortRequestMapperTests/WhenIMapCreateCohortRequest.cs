using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequest
    {
        private CreateCohortRequestMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private long _accountLegalEntityId;
        private Func<Task<CreateCohortRequest>> _act;
        private AccountLegalEntityResponse _accountLegalEntityResponse;


        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _accountLegalEntityId = fixture.Create<long>();
            _accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockCommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(_accountLegalEntityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountLegalEntityResponse);

            var birthDate = fixture.Create<DateTime?>();
            var startDate = fixture.Create<DateTime?>();
            var employmentEndDate = fixture.Create<DateTime?>();
            var endDate = fixture.Create<DateTime?>();
            var deliveryModel = fixture.Create<DeliveryModel?>();
            var employmentPrice = fixture.Create<int?>();
            var accountLegalEntityPublicHashedId = fixture.Create<string>();

            _mapper = new CreateCohortRequestMapper(_mockCommitmentsApiClient.Object);

            _source = fixture.Build<AddDraftApprenticeshipViewModel>()
                .With(x => x.EmployerAccountLegalEntityPublicHashedId, accountLegalEntityPublicHashedId)
                .With(x => x.AccountLegalEntityId, _accountLegalEntityId)
                .With(x => x.BirthDay, birthDate?.Day)
                .With(x => x.BirthMonth, birthDate?.Month)
                .With(x => x.BirthYear, birthDate?.Year)
                .With(x => x.EmploymentEndMonth, employmentEndDate?.Month)
                .With(x => x.EmploymentEndYear, employmentEndDate?.Year)
                .With(x => x.EndMonth, endDate?.Month)
                .With(x => x.EndYear, endDate?.Year)
                .With(x => x.StartMonth, startDate?.Month)
                .With(x => x.StartYear, startDate?.Year)
                .With(x => x.DeliveryModel, deliveryModel)
                .With(x => x.EmploymentPrice, employmentPrice)
                .With(x => x.IsOnFlexiPaymentPilot, true)
                .Without(x => x.StartDate)
                .Without(x => x.Courses)
                .Create();

            _act = () => _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Email, result.Email);
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DateOfBirth.Date, result.DateOfBirth);
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Uln, result.UniqueLearnerNumber);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Cost, result.Cost);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartDate.Date, result.StartDate);
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ActualStartDate.Date, result.ActualStartDate);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentEndDate.Date, result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndDate.Date, result.EndDate);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Reference, result.OriginatorReference);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenAccountIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountLegalEntityResponse.AccountId, result.AccountId);
        }

        [Test]
        public void AndWhenTheAccountLegalEntityIsNotFoundThenShouldThrowInvalidOperationException()
        {
            _mockCommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AccountLegalEntityResponse) null);

            Assert.ThrowsAsync<Exception>(() => _act());
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentPrice, result.EmploymentPrice);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
        }

        [Test]
        public async Task ThenIsProviderOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.IsProviderOnFlexiPaymentPilot, result.IsProviderOnFlexiPaymentPilot);
        }
    }
}
