﻿using System;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class EndDateRequestMapperTests
    {
        private EndDateRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new EndDateRequestMapperFixture();
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
        public async Task ThenStartDateIsPersistedToCache()
        {
            await _fixture.Act();
            _fixture.VerifyStartDatePersisted();
        }
    }

    public class EndDateRequestMapperFixture
    {
        private readonly Fixture _fixture;
        private readonly EndDateRequestMapper _sut;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        private readonly ChangeEmployerCacheItem _cacheItem;
        
        public StartDateViewModel ViewModel { get; }

        public EndDateRequestMapperFixture()
        {
            _fixture = new Fixture();

            ViewModel = new StartDateViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                StartMonth = 6,
                StartYear = 2020,
            };

            _cacheItem = _fixture.Create<ChangeEmployerCacheItem>();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == ViewModel.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _sut = new EndDateRequestMapper(_cacheStorage.Object);
        }

        public Task<EndDateRequest> Act() => _sut.Map(ViewModel);

        public void VerifyStartDatePersisted()
        {
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(key => key == _cacheItem.Key),
                It.Is<ChangeEmployerCacheItem>(cache => cache.StartDate == ViewModel.StartDate.MonthYear),
                It.IsAny<int>()));
        }
    }
}