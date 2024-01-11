using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ChangeOfEmployerOverlapAlertRequestMapperTests
    {
        private ChangeOfEmployerOverlapAlertRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ChangeOfEmployerOverlapAlertRequestMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenCacheKeyIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.CacheKey, result.CacheKey);
        }


        [Test]
        public async Task ThenPricesArePersistedToCache()
        {
            await _fixture.Act();
            _fixture.VerifyPricesPersisted();
        }
    }

    public class ChangeOfEmployerOverlapAlertRequestMapperFixture
    {
        private readonly Fixture _fixture;
        private readonly ChangeOfEmployerOverlapAlertRequestMapper _sut;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        private readonly ChangeEmployerCacheItem _cacheItem;

        public PriceViewModel ViewModel { get; }

        public ChangeOfEmployerOverlapAlertRequestMapperFixture()
        {
            _fixture = new Fixture();

            ViewModel = new PriceViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                Price = 2000,
                EmploymentPrice = 1000,
                CacheKey = Guid.NewGuid()
            };

            _cacheItem = _fixture.Create<ChangeEmployerCacheItem>();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == ViewModel.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _sut = new ChangeOfEmployerOverlapAlertRequestMapper(_cacheStorage.Object);
        }

        public Task<ChangeOfEmployerOverlapAlertRequest> Act() => _sut.Map(ViewModel);

        public void VerifyPricesPersisted()
        {
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.Key),
                It.Is<ChangeEmployerCacheItem>(cache => cache.Price == ViewModel.Price
                                                        && cache.EmploymentPrice == ViewModel.EmploymentPrice),
                It.IsAny<int>()));
        }
    }
}