﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using ChoosePilotStatusOptions = SFA.DAS.ProviderCommitments.Web.Models.ChoosePilotStatusOptions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class SelectPilotStatusViewModelMapperTests
    {
        private SelectPilotStatusViewModelMapper _mapper;
        private SelectPilotStatusRequest _request;
        private Mock<ICacheStorageService> _cacheStorage;
        private CreateCohortCacheItem _cacheItem;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<SelectPilotStatusRequest>();

            _cacheItem = fixture.Create<CreateCohortCacheItem>();

            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<CreateCohortCacheItem>(It.Is<Guid>(key => key == _request.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new SelectPilotStatusViewModelMapper(_cacheStorage.Object);
        }

        [TestCase(true, ChoosePilotStatusOptions.Pilot)]
        [TestCase(false, ChoosePilotStatusOptions.NonPilot)]
        [TestCase(null, null)]
        public async Task ThenPilotStatusIsMappedCorrectly(bool? pilotStatus, ChoosePilotStatusOptions? expectedOption)
        {
            _cacheItem.IsOnFlexiPaymentPilot = pilotStatus;
            var result = await _mapper.Map(_request);
            Assert.AreEqual(expectedOption, result.Selection);
        }

        [Test]
        public async Task ThenCacheKeyIsMappedCorrectly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.CacheKey, result.CacheKey);
        }

        [Test]
        public async Task ThenIsEditIsMappedCorrectly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.IsEdit, result.IsEdit);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_request.ProviderId, result.ProviderId);
        }
    }
}