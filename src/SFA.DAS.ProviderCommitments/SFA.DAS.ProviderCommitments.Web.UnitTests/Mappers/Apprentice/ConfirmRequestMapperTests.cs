using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmRequestMapperTests
    {
        private ConfirmRequestMapper _mapper;
        private PriceViewModel _source;
        private Func<Task<ConfirmRequest>> _act;
        private Mock<ICacheStorageService> _cacheStorage;
        private ChangeEmployerCacheItem _cacheItem;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<PriceViewModel>();

            _cacheItem = fixture.Create<ChangeEmployerCacheItem>();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(k => k == _source.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new ConfirmRequestMapper(Mock.Of<ILogger<ConfirmRequestMapper>>(), _cacheStorage.Object);

            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenProviderIdMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenPriceIsPersistedToCache()
        {
            await _act();
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.Key),
                It.Is<ChangeEmployerCacheItem>(i => i.Price == _source.Price),
                It.IsAny<int>()));
        }

        [Test]
        public async Task ThenEmploymentPriceIsPersistedToCache()
        {
            await _act();
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.Key),
                It.Is<ChangeEmployerCacheItem>(i => i.EmploymentPrice == _source.EmploymentPrice),
                It.IsAny<int>()));
        }
    }
}
