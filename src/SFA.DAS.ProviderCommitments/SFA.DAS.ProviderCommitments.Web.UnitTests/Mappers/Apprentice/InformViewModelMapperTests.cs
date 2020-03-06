using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class InformViewModelMapperTests
    {
        private GetInformPageFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetInformPageFixture();
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenApprenticeshipIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }
    }

    internal class GetInformPageFixture : Fixture
    {
        private readonly InformRequest _informRequest;
        private readonly InformViewModelMapper _sut;

        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }

        public GetInformPageFixture()
        {
            ProviderId = 123;
            ApprenticeshipId = 234;
            ApprenticeshipHashedId = "SD23DS24";
            _informRequest = new InformRequest
            {
                ApprenticeshipId = ApprenticeshipId,
                ProviderId = ProviderId,
                ApprenticeshipHashedId = ApprenticeshipHashedId
            };
            _sut = new InformViewModelMapper();
        }

        public Task<InformViewModel> Act() => _sut.Map(_informRequest);
    }
}