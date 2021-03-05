using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

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
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenStartDateIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.StartDate.MonthYear, result.StartDate);
        }
    }

    public class EndDateRequestMapperFixture
    {
        private readonly EndDateRequestMapper _sut;
        
        public StartDateViewModel ViewModel { get; }

        public EndDateRequestMapperFixture()
        {
            ViewModel = new StartDateViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                EmployerAccountLegalEntityPublicHashedId = "DFE348FD",
                StartMonth = 6,
                StartYear = 2020,
            };
            _sut = new EndDateRequestMapper();
        }

        public Task<EndDateRequest> Act() => _sut.Map(ViewModel);
    }
}