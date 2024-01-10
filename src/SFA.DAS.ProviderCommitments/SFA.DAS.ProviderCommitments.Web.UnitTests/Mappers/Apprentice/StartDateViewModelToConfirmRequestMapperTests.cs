using System;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class StartDateViewModelToConfirmRequestMapperTests
    {
        private StartDateViewModelToConfirmRequestMapper _mapper;
        private StartDateViewModel _source;
        private Func<Task<ConfirmRequest>> _act;
        private Mock<ICacheStorageService> _cacheStorage;
        private ChangeEmployerCacheItem _cacheItem;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<StartDateViewModel>().With(x=>x.StartDate, new MonthYearModel("042020")).Create();

            _cacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "042022")
                .Create();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(k => k == _source.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new StartDateViewModelToConfirmRequestMapper(Mock.Of<ILogger<StartDateViewModelToConfirmRequestMapper>>(), _cacheStorage.Object);

            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenProviderIdMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_source.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ThenStartDateIsPersistedToCache()
        {
            await _act();
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(k => k == _cacheItem.Key),
                It.Is<ChangeEmployerCacheItem>(c => c.StartDate == _source.StartDate.MonthYear),
                It.IsAny<int>()));
        }
    }
}
