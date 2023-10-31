using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.ChangeEmployer
{
    [TestFixture]
    public class SelectDeliveryModelViewModelToConfirmRequestMapperTests
    {
        private SelectDeliveryModelViewModelToConfirmRequestMapper _mapper;
        private Mock<ICacheStorageService> _cacheStorage;
        private ChangeEmployerCacheItem _cacheItem;
        private SelectDeliveryModelViewModel _request;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<SelectDeliveryModelViewModel>();
            
            _cacheItem = _fixture.Create<ChangeEmployerCacheItem>();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(k => k == _request.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _cacheStorage.Setup(x => x.SaveToCache(It.Is<Guid>(k => k == _request.CacheKey),
                    It.IsAny<ChangeEmployerCacheItem>(), It.IsAny<int>()))
                .Returns(() => Task.CompletedTask);

            _mapper = new SelectDeliveryModelViewModelToConfirmRequestMapper(_cacheStorage.Object, Mock.Of<ILogger<SelectDeliveryModelViewModelToConfirmRequestMapper>>());
        }

        [Test]
        public async Task DeliveryModel_Is_Persisted_To_Cache()
        {
            await _mapper.Map(_request);
            Assert.AreEqual(_request.DeliveryModel, _cacheItem.DeliveryModel);
        }

        [Test]
        public async Task ConfirmRequest_Is_Returned()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.CacheKey, result.CacheKey);
            Assert.AreEqual(_request.ProviderId, result.ProviderId);
            Assert.AreEqual(_request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }
    }
}
