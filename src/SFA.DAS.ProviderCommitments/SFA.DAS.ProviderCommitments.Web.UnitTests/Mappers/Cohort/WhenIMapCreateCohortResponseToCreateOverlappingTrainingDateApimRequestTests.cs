using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapCreateCohortResponseToCreateOverlappingTrainingDateApimRequestTests
    {
        private CreateCohortResponseToCreateOverlappingTrainingDateRequestMapper _mapper;
        private CreateCohortResponse _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _mapper = new CreateCohortResponseToCreateOverlappingTrainingDateRequestMapper();

            _source = fixture.Build<CreateCohortResponse>()
                .Create();
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.DraftApprenticeshipId, Is.EqualTo(_source.DraftApprenticeshipId));
        }
    }
}
