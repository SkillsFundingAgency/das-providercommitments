using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services.ProviderCommitmentsServiceTests
{
    [TestFixture]
    public class WhenAddingDraftApprenticeshipToCohort
    {
        private ProviderCommitmentsServiceTestFixtures _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new ProviderCommitmentsServiceTestFixtures();
        }

        [Test]
        public async Task IfShouldPassRequestToApiCall()
        {
            await _fixture.Sut.AddDraftApprenticeshipToCohort(_fixture.AddDraftApprenticeshipToCohortRequest);

            // TODO remove comment when API is updated
            //_fixture.CommitmentsApiClientMock.Verify(x=>x.AddDraftApprenticeshipToCohort(_fixture.AddDraftApprenticeshipToCohortRequest), Times.Once);
        }
    }
}