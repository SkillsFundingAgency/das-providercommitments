using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Commitments.Shared.UnitTests.Services.CommitmentsServiceTests
{
    [TestFixture]
    [Parallelizable]
    public class WhenGettingApprenticeships
    {
        private CommitmentsServiceTestFixtures _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new CommitmentsServiceTestFixtures();
            _fixture.SetupGetApprenticeshipsReturnValue(_fixture.GetApprenticeshipsResponse);
        }

        [Test]
        public async Task ShouldCallClientApiWithCorrectParameters()
        {
            await _fixture.Sut.GetApprenticeships(1, "sortField", true, true);

            _fixture.CommitmentsApiClientMock.Verify(x => x.GetApprenticeships(1, "sortField",true, true, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}