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
        public async Task ThenDraftAppretniceshipIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.DraftApprenticeshipId, result.DraftApprenticeshipId);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }
    }
}
