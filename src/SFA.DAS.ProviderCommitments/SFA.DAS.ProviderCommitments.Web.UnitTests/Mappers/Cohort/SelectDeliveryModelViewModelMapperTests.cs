using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class SelectDeliveryModelViewModelMapperTests
    {
        private SelectDeliveryModelViewModelMapper _mapper;

        private Mock<IOuterApiClient> _apiClient;
        private Mock<ICacheStorageService> _cacheStorageService;
        private CreateCohortWithDraftApprenticeshipRequest _request;
        private GetAddDraftApprenticeshipDeliveryModelResponse _apiResponse;
        private readonly Fixture _fixture = new();
        private CreateCohortCacheItem _cacheItem;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetAddDraftApprenticeshipDeliveryModelResponse>();
            _cacheItem = new CreateCohortCacheItem(Guid.NewGuid());

            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheItem);

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDeliveryModelResponse>(It.Is<GetAddDraftApprenticeshipDeliveryModelRequest>(r =>
                    r.AccountLegalEntityId == _cacheItem.AccountLegalEntityId
                    && r.CourseCode == _cacheItem.CourseCode
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new SelectDeliveryModelViewModelMapper(_apiClient.Object, _cacheStorageService.Object);
        }

        [Test]
        public async Task EmployerName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.That(result.EmployerName, Is.EqualTo(_apiResponse.EmployerName));
        }

        [Test]
        public async Task DeliveryModels_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.That(result.DeliveryModels, Is.EqualTo(_apiResponse.DeliveryModels));
        }

        [Test]
        public async Task DeliveryModel_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.That(result.DeliveryModel, Is.EqualTo(_cacheItem.DeliveryModel));
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task When_Only_One_Delivery_Model_Is_Available_It_Is_Saved_To_Cache(DeliveryModel selection)
        {
            _apiResponse.DeliveryModels.Clear();
            _apiResponse.DeliveryModels.Add(selection);

            await _mapper.Map(_request);

            _cacheStorageService.Setup(x =>
                x.SaveToCache<ICacheModel>(
                    It.Is<CreateCohortCacheItem>(m => m.DeliveryModel == selection), It.IsAny<int>()));
        }
    }
}
