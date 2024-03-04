using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapDraftApprenticeshipOverlapOptionViewModelToCreateOverlappingTrainingDateApimRequest
    {
        private DraftApprenticeshipOverlapOptionViewModelToCreateOverlappingTrainingDateRequestMapper _mapper;
        private DraftApprenticeshipOverlapOptionViewModel _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _mapper = new DraftApprenticeshipOverlapOptionViewModelToCreateOverlappingTrainingDateRequestMapper();

            _source = fixture.Build<DraftApprenticeshipOverlapOptionViewModel>()
                .Create();
        }

        [Test]
        public async Task ThenDraftApprenticeshipIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.DraftApprenticeshipId, Is.EqualTo(_source.DraftApprenticeshipId));
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }
    }
}
