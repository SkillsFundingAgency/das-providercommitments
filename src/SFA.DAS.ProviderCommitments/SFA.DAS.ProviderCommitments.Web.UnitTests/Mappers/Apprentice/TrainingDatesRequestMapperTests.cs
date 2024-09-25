using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class TrainingDatesRequestMapperTests
    {
        private TrainingDatesRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new TrainingDatesRequestMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            result.ApprenticeshipHashedId.Should().Be(_fixture.ViewModel.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            result.ProviderId.Should().Be(_fixture.ViewModel.ProviderId);
        }

        [Test]
        public async Task ThenCacheKeyIsMapped()
        {
            var result = await _fixture.Act();

            result.CacheKey.Should().Be(_fixture.ViewModel.CacheKey);
        }

        [Test]
        public async Task ThenEditIsMapped()
        {
            var result = await _fixture.Act();

            result.IsEdit.Should().Be(_fixture.ViewModel.IsEdit);
        }

        [Test]
        public async Task ThenDeliveryModelIsPersistedToCache()
        {
            await _fixture.Act();
            _fixture.VerifyDeliveryModelPersisted();
        }
    }

    public class TrainingDatesRequestMapperFixture
    {
        private readonly Fixture _fixture;
        private readonly TrainingDatesRequestMapper _sut;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        private readonly ChangeEmployerCacheItem _cacheItem;

        public SelectDeliveryModelViewModel ViewModel { get; }

        public TrainingDatesRequestMapperFixture()
        {
            _fixture = new Fixture();
            _cacheItem = _fixture.Create<ChangeEmployerCacheItem>();

            ViewModel = new SelectDeliveryModelViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                IsEdit = false,
                CacheKey = _cacheItem.CacheKey
            };
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == ViewModel.CacheKey)))
                .ReturnsAsync(_cacheItem);


            _sut = new TrainingDatesRequestMapper(_cacheStorage.Object);
        }

        public Task<TrainingDatesRequest> Act() => _sut.Map(ViewModel);

        public void VerifyDeliveryModelPersisted()
        {
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.CacheKey),
                It.Is<ChangeEmployerCacheItem>(cache => cache.DeliveryModel == ViewModel.DeliveryModel
                ),
                It.IsAny<int>()));
        }
    }
}