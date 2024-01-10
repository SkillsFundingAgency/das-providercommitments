using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapCreateCohortWithDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
    {
        private AddDraftApprenticeshipViewModelMapper _mapper;
        private CreateCohortWithDraftApprenticeshipRequest _source;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ICacheStorageService> _cacheService;
        private CreateCohortCacheItem _cacheItem;
        private GetAddDraftApprenticeshipDetailsResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _source = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>()
                .With(x => x.StartMonthYear, "042020")
                .Create();

            _apiResponse = fixture.Create<GetAddDraftApprenticeshipDetailsResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDetailsResponse>(It.IsAny<GetAddDraftApprenticeshipDetailsRequest>()))
                .ReturnsAsync(_apiResponse);

            _cacheItem = fixture.Build<CreateCohortCacheItem>()
                .With(x => x.StartMonthYear, "042020")
                .Create();
            _cacheService = new Mock<ICacheStorageService>();
            _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheItem);


            _mapper = new AddDraftApprenticeshipViewModelMapper(_apiClient.Object, _cacheService.Object);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.AccountLegalEntityId, Is.EqualTo(_cacheItem.AccountLegalEntityId));
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EmployerAccountLegalEntityPublicHashedId, Is.EqualTo(_source.EmployerAccountLegalEntityPublicHashedId));
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Employer, Is.EqualTo(_apiResponse.LegalEntityName));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.CourseCode, Is.EqualTo(_cacheItem.CourseCode));
        }

        [Test]
        public async Task ThenStartMonthYearIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(_source.StartMonthYear, Is.EqualTo(_cacheItem.StartMonthYear));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ReservationId, Is.EqualTo(_cacheItem.ReservationId));
        }

        [Test]
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.HasMultipleDeliveryModelOptions, Is.EqualTo(_apiResponse.HasMultipleDeliveryModelOptions));
        }

        [Test]
        public async Task ThenPilotStatusIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_cacheItem.IsOnFlexiPaymentPilot));
        }

        [Test]
        public async Task ThenUlnIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Uln, Is.EqualTo(_cacheItem.Uln));
        }

        [Test]
        public async Task Then_StartDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.StartDate.Date.Value.Date, Is.EqualTo(_cacheItem.StartDate.GetFirstDayOfMonth()));
        }

        [Test]
        public async Task Then_EndDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EndDate.Date.Value.Date, Is.EqualTo(_cacheItem.EndDate.Value.Date));
        }

        [Test]
        public async Task Then_ActualStartDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ActualStartDate.Date, Is.EqualTo(_cacheItem.ActualStartDate.Value.Date));
        }

        [Test]
        public async Task Then_EmploymentEndDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EmploymentEndDate.Date.Value.Date, Is.EqualTo(_cacheItem.EmploymentEndDate.GetFirstDayOfMonth()));
        }

        [Test]
        public async Task Then_EmploymentPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EmploymentPrice, Is.EqualTo(_cacheItem.EmploymentPrice));
        }


        [Test]
        public async Task Then_Cost_IsMappedCorrectly()
        {
            _cacheItem.IsOnFlexiPaymentPilot = false;
            var result = await _mapper.Map(_source);
            Assert.That(result.Cost, Is.EqualTo(_cacheItem.Cost));
        }

        [Test]
        public async Task Then_TrainingPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.TrainingPrice, Is.EqualTo(_cacheItem.TrainingPrice));
        }

        [Test]
        public async Task Then_EndPointAssessmentPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(_cacheItem.EndPointAssessmentPrice));
        }

        [Test]
        public async Task Then_Calculated_Cost_IsMappedCorrectly_ForPilotProviders()
        {
            _cacheItem.IsOnFlexiPaymentPilot = true;
            var result = await _mapper.Map(_source);
            Assert.That(result.Cost, Is.EqualTo(_cacheItem.TrainingPrice + _cacheItem.EndPointAssessmentPrice));
        }

        [Test]
        public async Task Then_Reference_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Reference, Is.EqualTo(_cacheItem.Reference));
        }


        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Email, Is.EqualTo(_cacheItem.Email));
        }
    }
}