using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapCreateCohortWithDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
    {
        private AddDraftApprenticeshipViewModelMapper _mapper;
        private CreateCohortWithDraftApprenticeshipRequest _source;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ITempDataStorageService> _tempData;
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

            _tempData = new Mock<ITempDataStorageService>();
            _tempData.Setup(x => x.RetrieveFromCache<AddDraftApprenticeshipViewModel>())
                .Returns(() => null);

            _cacheItem = fixture.Build<CreateCohortCacheItem>()
                .With(x => x.StartMonthYear, "042020")
                .Create();
            _cacheService = new Mock<ICacheStorageService>();
            _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheItem);


            _mapper = new AddDraftApprenticeshipViewModelMapper(_apiClient.Object, _tempData.Object, _cacheService.Object);
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
            Assert.AreEqual(_cacheItem.StartMonthYear, result.StartDate.MonthYear);
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
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_cacheItem.Email, result.Email);
        }
    }
}