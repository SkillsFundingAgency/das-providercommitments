using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class TrainingDatesViewModelToConfirmRequestMapperTests
    {
        private TrainingDatesViewModelToConfirmRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new TrainingDatesViewModelToConfirmRequestMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_fixture.ViewModel.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.ProviderId, Is.EqualTo(_fixture.ViewModel.ProviderId));
        }

        [Test]
        public async Task ThenCacheKeyIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.CacheKey, Is.EqualTo(_fixture.ViewModel.CacheKey));
        }

        [Test]
        public async Task ThenDatesArePersistedToCache()
        {
            await _fixture.Act();
            _fixture.VerifyDatesPersisted();
        }
    }

    public class TrainingDatesViewModelToConfirmRequestMapperFixture
    {
        private readonly Fixture _fixture;
        private readonly TrainingDatesViewModelToConfirmRequestMapper _sut;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        private readonly ChangeEmployerCacheItem _cacheItem;

        public TrainingDatesViewModel ViewModel { get; }

        public TrainingDatesViewModelToConfirmRequestMapperFixture()
        {
            _fixture = new Fixture();
            _cacheItem = _fixture.Create<ChangeEmployerCacheItem>();

            ViewModel = new TrainingDatesViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                CacheKey = _cacheItem.CacheKey,
                StartDate = new MonthYearModel(DateTime.Now.ToString("MMyyyy")),
                EndDate = new MonthYearModel(DateTime.Now.AddYears(1).ToString("MMyyyy"))
            };
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == ViewModel.CacheKey)))
                .ReturnsAsync(_cacheItem);


            _sut = new TrainingDatesViewModelToConfirmRequestMapper(_cacheStorage.Object);
        }

        public Task<ConfirmRequest> Act() => _sut.Map(ViewModel);

        public void VerifyDatesPersisted()
        {
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.CacheKey),
                It.Is<ChangeEmployerCacheItem>(cache => cache.StartDate == ViewModel.StartDate.MonthYear
                                                        && cache.EndDate == ViewModel.EndDate.MonthYear
                ),
                It.IsAny<int>()));
        }
    }
}