using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort.CreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequest
    {
        private CreateCohortRequestFromAddDraftApprenticeshipViewModelMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
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

            _cacheItem = fixture.Build<CreateCohortCacheItem>()
                .With(x => x.AccountLegalEntityId,
                    _accountLegalEntityId)
                .Create();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(_source.CacheKey))
                .ReturnsAsync(_cacheItem);

            _mapper = new CreateCohortRequestFromAddDraftApprenticeshipViewModelMapper(_mockCommitmentsApiClient.Object, _cacheStorageService.Object);

            _act = () => _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ReservationId, Is.EqualTo(_cacheItem.ReservationId));
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.FirstName, Is.EqualTo(_source.FirstName));
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Email, Is.EqualTo(_source.Email));
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DateOfBirth, Is.EqualTo(_source.DateOfBirth.Date));
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.UniqueLearnerNumber, Is.EqualTo(_source.Uln));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            _source.IsOnFlexiPaymentPilot = false;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.Cost));
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.TrainingPrice, Is.EqualTo(_source.TrainingPrice));
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(_source.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenCalculatedCostIsMappedCorrectlyForPilotProviders()
        {
            _source.IsOnFlexiPaymentPilot = true;
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_source.TrainingPrice + _source.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.StartDate, Is.EqualTo(_source.StartDate.Date));
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ActualStartDate, Is.EqualTo(_source.ActualStartDate.Date));
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentEndDate, Is.EqualTo(_source.EmploymentEndDate.Date));
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndDate, Is.EqualTo(_source.EndDate.Date));
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.OriginatorReference, Is.EqualTo(_source.Reference));
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.AccountLegalEntityId, Is.EqualTo(_accountLegalEntityId));
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenAccountIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.AccountId, Is.EqualTo(_accountLegalEntityResponse.AccountId));
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
            Assert.That(result.DeliveryModel, Is.EqualTo(_source.DeliveryModel));
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentPrice, Is.EqualTo(_source.EmploymentPrice));
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_source.IsOnFlexiPaymentPilot));
        }
    }
}
