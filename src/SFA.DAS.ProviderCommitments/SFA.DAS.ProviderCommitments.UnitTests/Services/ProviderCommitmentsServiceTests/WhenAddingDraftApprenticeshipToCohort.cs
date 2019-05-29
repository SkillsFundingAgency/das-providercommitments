using System.Threading;
using System.Threading.Tasks;
using Moq;
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
            await _fixture.Sut.AddDraftApprenticeshipToCohort(_fixture.CohortId, _fixture.AddDraftApprenticeshipRequest);

            _fixture.CommitmentsApiClientMock.Verify(x=>x.AddDraftApprenticeship(_fixture.CohortId, _fixture.AddDraftApprenticeshipRequest, CancellationToken.None), Times.Once);
        }
    }
}