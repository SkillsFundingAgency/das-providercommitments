using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.ProviderCommitments.Interfaces;

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
    }

    public class PriceRequestMapperFixture
    {
        private readonly PriceRequestMapper _sut;
        private Mock<ICacheStorageService> _cacheStorage;
        
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

            _cacheStorage = new Mock<ICacheStorageService>();

            _sut = new PriceRequestMapper(_cacheStorage.Object);
        }

        public Task<PriceRequest> Act() => _sut.Map(ViewModel);
    }
}