using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services.ProviderCommitmentsServiceTests
{
    [TestFixture]
    public class WhenUpdatingDraftApprenticeship
    {
        private ProviderCommitmentsServiceTestFixtures _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new ProviderCommitmentsServiceTestFixtures();
        }

        [Test]
        public async Task IfShouldForwardRequestToApiCall()
        {
            var updateRequest = new UpdateDraftApprenticeshipRequest();
            await _fixture.Sut.UpdateDraftApprenticeship(1, 2, updateRequest);

            _fixture.CommitmentsApiClientMock.Verify(x=>x.UpdateDraftApprenticeship(1,2, updateRequest, CancellationToken.None), Times.Once);
        }
    }
}