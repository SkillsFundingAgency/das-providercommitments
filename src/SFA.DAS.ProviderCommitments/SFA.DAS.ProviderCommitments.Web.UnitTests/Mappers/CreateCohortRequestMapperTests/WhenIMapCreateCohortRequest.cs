using System;
using System.Runtime.Caching;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequest
    {
        private CreateCohortRequestMapper _mapper;
        private AddDraftApprenticeshipOrRoutePostRequest _source;
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private Mock<ICacheStorageService> _cacheStorageService;
        private long _accountLegalEntityId;
        private Func<Task<CreateCohortRequest>> _act;
        private AccountLegalEntityResponse _accountLegalEntityResponse;
        private CreateCohortCacheItem _cacheItem;


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

            _source = fixture.Build<AddDraftApprenticeshipOrRoutePostRequest>()
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

            _cacheItem = fixture.Build<CreateCohortCacheItem>()
                .With(x => x.AccountLegalEntityId,
                    _accountLegalEntityId)
                .Create();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(_source.CacheKey))
                .ReturnsAsync(_cacheItem);

            _mapper = new CreateCohortRequestMapper(_mockCommitmentsApiClient.Object, _cacheStorageService.Object);

            _act = () => _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_cacheItem.ReservationId, result.ReservationId);
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
            _source.IsOnFlexiPaymentPilot = false;
            var result = await _act();
            Assert.AreEqual(_source.Cost, result.Cost);
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice, result.TrainingPrice);
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndPointAssessmentPrice, result.EndPointAssessmentPrice);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectlyForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            var result = await _act();
            Assert.AreEqual(_source.TrainingPrice + _source.EndPointAssessmentPrice, result.Cost);
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
    }
}
