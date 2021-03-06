﻿using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class PriceRequestMapperTests
    {
        private PriceRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new PriceRequestMapperFixture();
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
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenStartDateIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.StartDate, result.StartDate);
        }

        [Test]
        public async Task ThenEndDateIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.EndDate.MonthYear, result.EndDate);
        }
    }

    public class PriceRequestMapperFixture
    {
        private readonly PriceRequestMapper _sut;
        
        public EndDateViewModel ViewModel { get; }

        public PriceRequestMapperFixture()
        {
            ViewModel = new EndDateViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                EmployerAccountLegalEntityPublicHashedId = "DFE348FD",
                StartDate = "012020",
                EndMonth = 6,
                EndYear = 2020,
            };
            _sut = new PriceRequestMapper();
        }

        public Task<PriceRequest> Act() => _sut.Map(ViewModel);
    }
}