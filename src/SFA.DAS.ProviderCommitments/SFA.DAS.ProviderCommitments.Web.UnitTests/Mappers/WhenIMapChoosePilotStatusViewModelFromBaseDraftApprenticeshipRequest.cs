using FluentAssertions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapChoosePilotStatusViewModelFromBaseDraftApprenticeshipRequest
    {
        private ChoosePilotStatusViewModelFromBaseDraftApprenticeshipRequestMapper _mapper;
        private BaseDraftApprenticeshipRequest _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<BaseDraftApprenticeshipRequest>();

            _mapper = new ChoosePilotStatusViewModelFromBaseDraftApprenticeshipRequestMapper();
        }

        [Test]
        public async Task ProviderIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.ProviderId.Should().Be(_source.ProviderId);
        }

        [Test]
        public async Task CohortReferenceIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.CohortReference.Should().Be(_source.CohortReference);
        }

        [Test]
        public async Task DraftApprenticeshipHashedIdIsMapped()
        {
            var result = await _mapper.Map(_source);
            result.DraftApprenticeshipHashedId.Should().Be(_source.DraftApprenticeshipHashedId);
        }
    }
}