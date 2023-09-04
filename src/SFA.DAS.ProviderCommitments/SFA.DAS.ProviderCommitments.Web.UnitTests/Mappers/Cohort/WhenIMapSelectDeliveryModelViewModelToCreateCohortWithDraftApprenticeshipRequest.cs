﻿using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using System;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelToCreateCohortWithDraftApprenticeshipRequest
    {
        private CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper _mapper;
        private SelectDeliveryModelViewModel _source;
        private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;
        private Mock<ICacheStorageService> _cacheService;
        private CreateCohortCacheItem _cacheItem;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<SelectDeliveryModelViewModel>();

            _cacheItem = fixture.Create<CreateCohortCacheItem>();
            _cacheService = new Mock<ICacheStorageService>();
            _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheItem);

            _mapper = new CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper(_cacheService.Object);

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartMonthYear, result.StartMonthYear);
        }
    }
}
