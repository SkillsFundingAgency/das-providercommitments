using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
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
            Assert.AreEqual(_cacheItem.AccountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.LegalEntityName, result.Employer);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenStartMonthYearIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.StartMonthYear, _source.StartMonthYear);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.HasMultipleDeliveryModelOptions, result.HasMultipleDeliveryModelOptions);
        }

        [Test]
        public async Task ThenPilotStatusIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
        }

        [Test]
        public async Task ThenUlnIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.Uln, result.Uln);
        }

        [Test]
        public async Task Then_StartDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.StartDate.GetFirstDayOfMonth(), result.StartDate.Date.Value.Date);
        }

        [Test]
        public async Task Then_EndDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.EndDate.Value.Date, result.EndDate.Date.Value.Date);
        }

        [Test]
        public async Task Then_ActualStartDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.ActualStartDate.Value.Date, result.ActualStartDate.Date);
        }

        [Test]
        public async Task Then_EmploymentEndDate_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.EmploymentEndDate.GetFirstDayOfMonth(), result.EmploymentEndDate.Date.Value.Date);
        }

        [Test]
        public async Task Then_EmploymentPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.EmploymentPrice, result.EmploymentPrice);
        }


        [Test]
        public async Task Then_Cost_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.Cost, result.Cost);
        }

        [Test]
        public async Task Then_TrainingPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.TrainingPrice, result.TrainingPrice);
        }

        [Test]
        public async Task Then_EndPointAssessmentPrice_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.EndPointAssessmentPrice, result.EndPointAssessmentPrice);
        }

        [Test]
        public async Task Then_Reference_IsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.Reference, result.Reference);
        }


        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.Email, result.Email);
        }
    }
}