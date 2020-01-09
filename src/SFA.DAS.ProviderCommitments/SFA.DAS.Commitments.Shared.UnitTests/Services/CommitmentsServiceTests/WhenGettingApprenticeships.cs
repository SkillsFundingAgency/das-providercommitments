using System.Threading.Tasks;
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
        }

        [Test]
        public async Task TheShouldReturnApprenticeshipsForProvider()
        {
            //Act
            var response = await _fixture.Sut.GetApprenticeships(1, 1, 1);

            //Assert
            Assert.AreEqual(_fixture.GetApprenticeshipsResponse.Apprenticeships, response.Apprenticeships);
        }

        [Test]
        public async Task TheShouldReturnTotalApprenticeshipCount()
        {
            //Act
            var response = await _fixture.Sut.GetApprenticeships(1, 1, 1);

            //Assert
            Assert.AreEqual(_fixture.GetApprenticeshipsResponse.TotalApprenticeshipsFound, response.TotalNumberOfApprenticeshipsFound);
        }

        [Test]
        public async Task TheShouldReturnTotalApprenticeshipWithAlertsCount()
        {
            //Act
            var response = await _fixture.Sut.GetApprenticeships(1, 1, 1);

            //Assert
            Assert.AreEqual(_fixture.GetApprenticeshipsResponse.TotalApprenticeshipsWithAlertsFound, response.TotalNumberOfApprenticeshipsWithAlertsFound);
        }
    }
}
