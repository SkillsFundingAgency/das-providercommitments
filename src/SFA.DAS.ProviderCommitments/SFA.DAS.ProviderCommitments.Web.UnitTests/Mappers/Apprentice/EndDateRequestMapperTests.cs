using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.ProviderCommitments.Interfaces;

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
    }

    public class EndDateRequestMapperFixture
    {
        private readonly EndDateRequestMapper _sut;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        
        public StartDateViewModel ViewModel { get; }

        public EndDateRequestMapperFixture()
        {
            ViewModel = new StartDateViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                StartMonth = 6,
                StartYear = 2020,
            };

            _cacheStorage = new Mock<ICacheStorageService>();

            _sut = new EndDateRequestMapper(_cacheStorage.Object);
        }

        public Task<EndDateRequest> Act() => _sut.Map(ViewModel);
    }
}